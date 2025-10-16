// ==================== ORDER TRACKING MAP JS ====================
// Global variables
let isEditingAddress = false;
let usedCurrentLocation = false;
let mapInitialized = false;

// ==================== MAP INITIALIZATION ====================
document.addEventListener('DOMContentLoaded', function() {
    // Wait for maps.js to initialize before setting up
    setTimeout(() => {
        // Rename inputs to match what maps.js expects
        const latInput = document.getElementById('latitudeInput');
        const lngInput = document.getElementById('longitudeInput');
        
        if (latInput) latInput.id = 'latitude';
        if (lngInput) lngInput.id = 'longitude';
        
        // Now initialize the map
        if (typeof initAddressMap === 'function') {
            initAddressMap('addressMap', 'latitude', 'longitude');
            setupCurrentLocationButton('currentLocationBtn');
        }
        
        // Setup search button after map is initialized
        setupSearchButton();
    }, 500);
    
    // Backup initialization
    window.addEventListener('load', function() {
        if (typeof geocoder === 'undefined') {
            setTimeout(() => setupSearchButton(), 1000);
        }
    });
    
    // Initialize order type toggle
    initializeOrderTypeToggle();
    
    // Initialize address selection
    initializeAddressSelection();
    
    // Initialize payment validation
    initializePaymentValidation();
});

// Check map initialization status
window.addEventListener('load', function() {
    setTimeout(() => {
        if (typeof map !== 'undefined' && typeof geocoder !== 'undefined') {
            mapInitialized = true;
        }
    }, 1000);
});

// ==================== ORDER TYPE TOGGLE ====================
function initializeOrderTypeToggle() {
    const orderTypeRadios = document.querySelectorAll('input[name="OrderType"]');
    const deliveryAddressSection = document.getElementById('deliveryAddressSection');
    const shippingAddressDisplay = document.getElementById('shippingAddressDisplay');
    const deliveryFeeAmount = document.getElementById('deliveryFeeAmount');
    const totalAmount = document.getElementById('totalAmount');
    const orderTypeBadge = document.getElementById('orderTypeBadge');

    if (!orderTypeRadios.length) return;

    function updateOrderType() {
        const selectedType = document.querySelector('input[name="OrderType"]:checked');
        if (!selectedType) return;
        
        const type = selectedType.value;
        if (orderTypeBadge) orderTypeBadge.textContent = type;

        // Get subtotal from window variable (set by Razor)
        const subtotal = window.checkoutSubtotal || 0;

        if (type === 'Pickup') {
            if (deliveryAddressSection) deliveryAddressSection.classList.add('hidden');
            if (shippingAddressDisplay) shippingAddressDisplay.classList.add('hidden');
            if (deliveryFeeAmount) deliveryFeeAmount.textContent = '₱0.00';
            if (totalAmount) totalAmount.textContent = '₱' + subtotal.toFixed(2);
        } else {
            if (deliveryAddressSection) deliveryAddressSection.classList.remove('hidden');
            if (shippingAddressDisplay) shippingAddressDisplay.classList.remove('hidden');
            if (deliveryFeeAmount) deliveryFeeAmount.textContent = '₱50.00';
            if (totalAmount) totalAmount.textContent = '₱' + (subtotal + 50).toFixed(2);
        }
    }

    orderTypeRadios.forEach(radio => {
        radio.addEventListener('change', updateOrderType);
    });

    // Initialize on page load
    updateOrderType();
}

// ==================== ADDRESS SELECTION ====================
function initializeAddressSelection() {
    const addressRadios = document.querySelectorAll('.address-radio');
    
    addressRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            if (this.checked) {
                updateSelectedAddress(this);
            }
        });
    });

    // Initialize with default address
    const defaultAddressRadio = document.querySelector('.address-radio:checked');
    if (defaultAddressRadio) {
        updateSelectedAddress(defaultAddressRadio);
    }
}

function updateSelectedAddress(radioElement) {
    const street = radioElement.getAttribute('data-street');
    const barangay = radioElement.getAttribute('data-barangay');
    const city = radioElement.getAttribute('data-city');
    const province = radioElement.getAttribute('data-province');
    const zipCode = radioElement.getAttribute('data-zipcode');
    const latitude = radioElement.getAttribute('data-latitude');
    const longitude = radioElement.getAttribute('data-longitude');
    
    // Update the order summary address
    const fullAddress = `${street}, ${barangay}, ${city}, ${province} ${zipCode}`;
    const summaryAddress = document.getElementById('summaryAddress');
    if (summaryAddress) summaryAddress.textContent = fullAddress;
    
    // Update map address text
    const mapAddressText = document.getElementById('mapAddressText');
    if (mapAddressText) mapAddressText.textContent = `${street}, ${barangay}, ${city}`;
    
    // Update form fields
    const streetInput = document.querySelector('input[name="Street"]');
    const barangayInput = document.querySelector('input[name="Barangay"]');
    const cityInput = document.querySelector('input[name="City"]');
    const provinceInput = document.querySelector('input[name="Province"]');
    const zipCodeInput = document.querySelector('input[name="ZipCode"]');
    
    if (streetInput) streetInput.value = street;
    if (barangayInput) barangayInput.value = barangay;
    if (cityInput) cityInput.value = city;
    if (provinceInput) provinceInput.value = province;
    if (zipCodeInput) zipCodeInput.value = zipCode;
    
    const latInput = document.getElementById('latitude') || document.getElementById('latitudeInput');
    const lngInput = document.getElementById('longitude') || document.getElementById('longitudeInput');
    if (latInput) latInput.value = latitude;
    if (lngInput) lngInput.value = longitude;
    
    // Update map if it's initialized
    if (typeof map !== 'undefined' && latitude && longitude) {
        const location = { 
            lat: parseFloat(latitude), 
            lng: parseFloat(longitude) 
        };
        
        map.setCenter(location);
        map.setZoom(17);
        
        if (typeof placeMarker === 'function') {
            placeMarker(location);
        }
        
        if (typeof updateCoordinatesDisplay === 'function') {
            updateCoordinatesDisplay(location.lat, location.lng);
        }
    }
}

// ==================== ADDRESS EDIT FORM ====================
const editAddressBtn = document.getElementById('editAddressBtn');
if (editAddressBtn) {
    editAddressBtn.addEventListener('click', function() {
        // Clear all address input fields
        const streetInput = document.querySelector('input[name="Street"]');
        const barangayInput = document.querySelector('input[name="Barangay"]');
        const cityInput = document.querySelector('input[name="City"]');
        const provinceInput = document.querySelector('input[name="Province"]');
        const zipCodeInput = document.querySelector('input[name="ZipCode"]');
        
        if (streetInput) streetInput.value = '';
        if (barangayInput) barangayInput.value = '';
        if (cityInput) cityInput.value = '';
        if (provinceInput) provinceInput.value = '';
        if (zipCodeInput) zipCodeInput.value = '';
        
        // Clear coordinates
        const latInput = document.getElementById('latitude') || document.getElementById('latitudeInput');
        const lngInput = document.getElementById('longitude') || document.getElementById('longitudeInput');
        if (latInput) latInput.value = '';
        if (lngInput) lngInput.value = '';
        
        // Reset flags
        usedCurrentLocation = false;
        
        // Update map text
        const mapAddressText = document.getElementById('mapAddressText');
        if (mapAddressText) {
            mapAddressText.textContent = 'Click on map or use buttons below to set location';
        }
        
        // Show edit form
        const addressDisplay = document.getElementById('addressDisplay');
        const addressEditForm = document.getElementById('addressEditForm');
        if (addressDisplay) addressDisplay.classList.add('hidden');
        if (addressEditForm) addressEditForm.classList.remove('hidden');
    });
}

const cancelEditBtn = document.getElementById('cancelEditBtn');
if (cancelEditBtn) {
    cancelEditBtn.addEventListener('click', function() {
        const addressDisplay = document.getElementById('addressDisplay');
        const addressEditForm = document.getElementById('addressEditForm');
        if (addressDisplay) addressDisplay.classList.remove('hidden');
        if (addressEditForm) addressEditForm.classList.add('hidden');
        usedCurrentLocation = false;
    });
}

const useAddressBtn = document.getElementById('useAddressBtn');
if (useAddressBtn) {
    useAddressBtn.addEventListener('click', function() {
        useCurrentMapLocation();
    });
}

// ==================== CURRENT LOCATION ====================
const currentLocationBtn = document.getElementById('currentLocationBtn');
if (currentLocationBtn) {
    currentLocationBtn.addEventListener('click', function() {
        useCurrentLocationForAddress();
    });
}

function useCurrentLocationForAddress() {
    if (!navigator.geolocation) {
        alert('Geolocation is not supported by your browser.');
        return;
    }
    
    const button = document.getElementById('currentLocationBtn');
    if (!button) return;
    
    const originalHTML = button.innerHTML;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Getting Location...';
    button.disabled = true;

    navigator.geolocation.getCurrentPosition(
        function (position) {
            const location = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            // Set flag that user used current location
            usedCurrentLocation = true;

            // Update coordinates immediately
            const latInput = document.getElementById('latitude') || document.getElementById('latitudeInput');
            const lngInput = document.getElementById('longitude') || document.getElementById('longitudeInput');
            if (latInput) latInput.value = location.lat;
            if (lngInput) lngInput.value = location.lng;

            // Use reverse geocoding to get address
            if (typeof geocoder !== 'undefined') {
                geocoder.geocode({ location: location }, (results, status) => {
                    button.innerHTML = originalHTML;
                    button.disabled = false;
                    
                    if (status === 'OK' && results[0]) {
                        const address = results[0];
                        
                        // Update map
                        if (typeof map !== 'undefined' && typeof placeMarker === 'function') {
                            map.setCenter(location);
                            map.setZoom(17);
                            placeMarker(location);
                            if (typeof updateCoordinatesDisplay === 'function') {
                                updateCoordinatesDisplay(location.lat, location.lng);
                            }
                        }
                        
                        // Update map address text
                        const mapAddressText = document.getElementById('mapAddressText');
                        if (mapAddressText) {
                            mapAddressText.textContent = address.formatted_address;
                        }
                        
                        alert('Current location captured! You can now click "Use Address" to save this location.');
                    } else {
                        const mapAddressText = document.getElementById('mapAddressText');
                        if (mapAddressText) {
                            mapAddressText.textContent = `Lat: ${location.lat.toFixed(6)}, Lng: ${location.lng.toFixed(6)}`;
                        }
                        alert('Location captured but could not get address details. You can still use this location.');
                    }
                });
            } else {
                button.innerHTML = originalHTML;
                button.disabled = false;
                alert('Location captured! You can now click "Use Address" to save this location.');
            }
        },
        function (error) {
            console.error('Geolocation error:', error);
            alert('Unable to get your current location. Please enable location services or enter address manually.');
            button.innerHTML = originalHTML;
            button.disabled = false;
        },
        {
            timeout: 10000,
            maximumAge: 60000,
            enableHighAccuracy: true
        }
    );
}

// ==================== SEARCH ADDRESS ====================
function setupSearchButton() {
    const searchBtn = document.getElementById('searchAddressBtn');
    if (!searchBtn) return;
    
    // Remove existing listeners
    const newSearchBtn = searchBtn.cloneNode(true);
    searchBtn.parentNode.replaceChild(newSearchBtn, searchBtn);
    
    newSearchBtn.addEventListener('click', function() {
        const streetInput = document.querySelector('input[name="Street"]');
        const barangayInput = document.querySelector('input[name="Barangay"]');
        const cityInput = document.querySelector('input[name="City"]');
        const provinceInput = document.querySelector('input[name="Province"]');
        
        const street = streetInput ? streetInput.value.trim() : '';
        const barangay = barangayInput ? barangayInput.value.trim() : '';
        const city = cityInput ? cityInput.value.trim() : '';
        const province = provinceInput ? provinceInput.value.trim() : '';
        
        // Validate input
        if (!street && !city && !barangay && !province) {
            alert('Please enter at least one address field (Street, Barangay, City, or Province) to search on the map.');
            return;
        }

        // Check if map is ready
        if (typeof geocoder === 'undefined' || typeof map === 'undefined') {
            alert('Map is still loading. Please wait a moment and try again.');
            return;
        }

        const addressParts = [street, barangay, city, province, 'Philippines'].filter(part => part);
        const address = addressParts.join(', ');
        
        const originalHTML = this.innerHTML;
        this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Searching...';
        this.disabled = true;

        // Geocode the address
        geocoder.geocode({ address: address }, (results, status) => {
            if (status === 'OK' && results[0]) {
                const location = results[0].geometry.location;
                
                // Update map
                if (typeof placeMarker === 'function') {
                    map.setCenter(location);
                    map.setZoom(17);
                    placeMarker(location);
                    
                    // Update coordinates
                    const latInput = document.getElementById('latitude');
                    const lngInput = document.getElementById('longitude');
                    if (latInput) latInput.value = location.lat();
                    if (lngInput) lngInput.value = location.lng();
                    
                    // Update map address text
                    const mapAddressText = document.getElementById('mapAddressText');
                    if (mapAddressText) {
                        mapAddressText.textContent = results[0].formatted_address;
                    }
                    
                    // Update coordinates display
                    if (typeof updateCoordinatesDisplay === 'function') {
                        updateCoordinatesDisplay(location.lat(), location.lng());
                    }
                }
                this.innerHTML = '<i class="fas fa-check"></i> Found!';
            } else {
                alert('Address not found on map. Please try different search terms or click on the map to set location manually.');
                this.innerHTML = '<i class="fas fa-times"></i> Not Found';
            }
            setTimeout(() => {
                this.innerHTML = originalHTML;
                this.disabled = false;
            }, 2000);
        });
    });
}

// ==================== USE ADDRESS ====================
function useCurrentMapLocation() {
    // Get coordinates
    const latInput = document.getElementById('latitude') || document.getElementById('latitudeInput');
    const lngInput = document.getElementById('longitude') || document.getElementById('longitudeInput');
    
    const latitude = latInput ? latInput.value : '';
    const longitude = lngInput ? lngInput.value : '';

    if (!latitude || !longitude) {
        alert('Please select a location on the map first (use Current Location button, Search on Map, or click directly on the map)');
        return;
    }

    // Validate address fields if not using current location
    if (!usedCurrentLocation) {
        const streetInput = document.querySelector('input[name="Street"]');
        const barangayInput = document.querySelector('input[name="Barangay"]');
        const cityInput = document.querySelector('input[name="City"]');
        const provinceInput = document.querySelector('input[name="Province"]');
        const zipCodeInput = document.querySelector('input[name="ZipCode"]');
        
        const street = streetInput ? streetInput.value.trim() : '';
        const barangay = barangayInput ? barangayInput.value.trim() : '';
        const city = cityInput ? cityInput.value.trim() : '';
        const province = provinceInput ? provinceInput.value.trim() : '';
        const zipCode = zipCodeInput ? zipCodeInput.value.trim() : '';

        if (!street || !barangay || !city || !province || !zipCode) {
            alert('Please fill in all address fields (Street, Barangay, City, Province, and Zip Code) or use the "Current Location" button.');
            return;
        }
    }

    // Get address for display
    const streetInput = document.querySelector('input[name="Street"]');
    const barangayInput = document.querySelector('input[name="Barangay"]');
    const cityInput = document.querySelector('input[name="City"]');
    const provinceInput = document.querySelector('input[name="Province"]');
    const zipCodeInput = document.querySelector('input[name="ZipCode"]');
    
    const street = streetInput ? streetInput.value.trim() : '';
    const barangay = barangayInput ? barangayInput.value.trim() : '';
    const city = cityInput ? cityInput.value.trim() : '';
    const province = provinceInput ? provinceInput.value.trim() : '';
    const zipCode = zipCodeInput ? zipCodeInput.value.trim() : '';

    // Create display address
    let displayAddress;
    if (street && barangay && city && province) {
        displayAddress = `${street}, ${barangay}, ${city}, ${province} ${zipCode}`;
    } else {
        const mapText = document.getElementById('mapAddressText')?.textContent;
        if (mapText && !mapText.includes('Click on map')) {
            displayAddress = mapText;
        } else {
            displayAddress = `Location: ${parseFloat(latitude).toFixed(6)}, ${parseFloat(longitude).toFixed(6)}`;
        }
    }

    // Update order summary
    const summaryAddress = document.getElementById('summaryAddress');
    if (summaryAddress) summaryAddress.textContent = displayAddress;

    // Add custom address container
    addCustomAddressContainer(displayAddress, latitude, longitude);

    // Close edit form
    const addressDisplay = document.getElementById('addressDisplay');
    const addressEditForm = document.getElementById('addressEditForm');
    if (addressDisplay) addressDisplay.classList.remove('hidden');
    if (addressEditForm) addressEditForm.classList.add('hidden');
    
    usedCurrentLocation = false;
}

function addCustomAddressContainer(address, latitude, longitude) {
    const addressSelection = document.getElementById('addressSelection');
    if (!addressSelection) return;
    
    // Remove existing custom address
    const existingCustom = document.getElementById('customAddressContainer');
    if (existingCustom) existingCustom.remove();

    // Create new container
    const customAddressContainer = document.createElement('div');
    customAddressContainer.id = 'customAddressContainer';
    customAddressContainer.className = 'mt-4 p-4 bg-green-50 border-2 border-green-200 rounded-xl';
    
    customAddressContainer.innerHTML = `
        <div class="flex items-start justify-between">
            <div class="flex-1">
                <div class="flex items-center gap-2 mb-2">
                    <span class="font-semibold text-green-800 text-sm">Selected Delivery Location</span>
                    <span class="bg-green-600 text-white text-xs px-2 py-0.5 rounded-full font-semibold">
                        Current
                    </span>
                </div>
                <p class="text-sm text-gray-700">${address}</p>
                <p class="text-xs text-gray-500 mt-1">
                    Coordinates: ${parseFloat(latitude).toFixed(6)}, ${parseFloat(longitude).toFixed(6)}
                </p>
            </div>
            <button type="button" onclick="removeCustomAddress()" class="text-gray-400 hover:text-red-500 transition-colors">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;

    addressSelection.parentNode.insertBefore(customAddressContainer, addressSelection.nextSibling);
}

function removeCustomAddress() {
    const customAddressContainer = document.getElementById('customAddressContainer');
    if (customAddressContainer) {
        customAddressContainer.remove();
    }
    usedCurrentLocation = false;
}

// ==================== PAYMENT VALIDATION ====================
function initializePaymentValidation() {
    const paymentMethods = document.querySelectorAll('input[name="PaymentMethod"]');
    const gcashFields = document.getElementById('gcashFields');
    const cardFields = document.getElementById('cardFields');

    function togglePaymentFields() {
        const selectedMethod = document.querySelector('input[name="PaymentMethod"]:checked');
        
        if (gcashFields) gcashFields.classList.add('hidden');
        if (cardFields) cardFields.classList.add('hidden');
        
        if (selectedMethod) {
            if (selectedMethod.value === 'GCash' && gcashFields) {
                gcashFields.classList.remove('hidden');
            } else if (selectedMethod.value === 'Card' && cardFields) {
                cardFields.classList.remove('hidden');
            }
        }
    }

    paymentMethods.forEach(method => {
        method.addEventListener('change', togglePaymentFields);
    });

    // Setup input validation
    setupGCashValidation();
    setupCardValidation();
    setupFormValidation();
    setupRealTimeValidation();
}

// GCash validation
function setupGCashValidation() {
    const gcashInput = document.getElementById('gcashNumberInput');
    if (!gcashInput) return;
    
    gcashInput.addEventListener('input', function(e) {
        this.value = this.value.replace(/\D/g, '');
        if (this.value.length > 11) {
            this.value = this.value.slice(0, 11);
        }
        const errorElement = document.getElementById('gcashError');
        if (errorElement) {
            if (this.value.length > 0 && this.value.length !== 11) {
                errorElement.classList.remove('hidden');
            } else {
                errorElement.classList.add('hidden');
            }
        }
    });
}

// Card validation
function setupCardValidation() {
    // Card Number
    const cardNumberInput = document.getElementById('cardNumberInput');
    if (cardNumberInput) {
        cardNumberInput.addEventListener('input', function(e) {
            let value = this.value.replace(/\D/g, '');
            if (value.length > 16) value = value.slice(0, 16);
            
            let formattedValue = '';
            for (let i = 0; i < value.length; i++) {
                if (i > 0 && i % 4 === 0) formattedValue += ' ';
                formattedValue += value[i];
            }
            this.value = formattedValue;
            
            const errorElement = document.getElementById('cardNumberError');
            if (errorElement) {
                const digitCount = value.length;
                if (digitCount > 0 && digitCount !== 16) {
                    errorElement.classList.remove('hidden');
                } else {
                    errorElement.classList.add('hidden');
                }
            }
        });
    }

    // Expiry Date
    const expiryInput = document.getElementById('cardExpiryInput');
    if (expiryInput) {
        expiryInput.addEventListener('input', function(e) {
            let value = this.value.replace(/\D/g, '');
            if (value.length > 4) value = value.slice(0, 4);
            if (value.length >= 2) {
                value = value.slice(0, 2) + '/' + value.slice(2);
            }
            this.value = value;
            
            const errorElement = document.getElementById('expiryError');
            if (errorElement) {
                const digitCount = value.replace(/\D/g, '').length;
                if (digitCount > 0 && digitCount !== 4) {
                    errorElement.classList.remove('hidden');
                } else {
                    errorElement.classList.add('hidden');
                }
                if (digitCount >= 2) {
                    const month = parseInt(value.slice(0, 2));
                    if (month < 1 || month > 12) {
                        errorElement.textContent = 'Month must be between 01-12';
                        errorElement.classList.remove('hidden');
                    } else {
                        errorElement.textContent = 'Please enter valid expiry date';
                    }
                }
            }
        });
    }

    // CVV
    const cvvInput = document.getElementById('cardCvvInput');
    if (cvvInput) {
        cvvInput.addEventListener('input', function(e) {
            this.value = this.value.replace(/\D/g, '');
            if (this.value.length > 3) this.value = this.value.slice(0, 3);
            
            const errorElement = document.getElementById('cvvError');
            if (errorElement) {
                if (this.value.length > 0 && this.value.length !== 3) {
                    errorElement.classList.remove('hidden');
                } else {
                    errorElement.classList.add('hidden');
                }
            }
        });
    }

    // Cardholder Name
    const nameInput = document.getElementById('cardholderNameInput');
    if (nameInput) {
        nameInput.addEventListener('input', function(e) {
            this.value = this.value.replace(/[^a-zA-Z\s\-']/g, '');
            const errorElement = document.getElementById('nameError');
            if (errorElement) {
                if (this.value.length === 0) {
                    errorElement.classList.remove('hidden');
                } else {
                    errorElement.classList.add('hidden');
                }
            }
        });
    }
}

// Form submission validation
function setupFormValidation() {
    const orderForm = document.querySelector('form[method="post"]');
    if (!orderForm) return;
    
    orderForm.addEventListener('submit', function(e) {
        if (!validatePaymentMethod()) {
            e.preventDefault();
            return false;
        }
    });
}

function validatePaymentMethod() {
    const selectedMethod = document.querySelector('input[name="PaymentMethod"]:checked');
    
    if (!selectedMethod) {
        alert('Please select a payment method.');
        return false;
    }

    const method = selectedMethod.value;

    // Cash - no validation needed
    if (method === 'Cash') return true;

    // GCash validation
    if (method === 'GCash') {
        const gcashNumber = document.getElementById('gcashNumberInput')?.value.replace(/\D/g, '') || '';
        
        if (!gcashNumber) {
            alert('Please enter your GCash number.');
            document.getElementById('gcashNumberInput')?.focus();
            return false;
        }
        
        if (gcashNumber.length !== 11) {
            alert('Please enter a valid 11-digit GCash number.');
            document.getElementById('gcashNumberInput')?.focus();
            return false;
        }
        
        if (!gcashNumber.startsWith('09')) {
            alert('GCash number must start with "09".');
            document.getElementById('gcashNumberInput')?.focus();
            return false;
        }
        
        return true;
    }

    // Card validation
    if (method === 'Card') {
        const cardNumber = document.getElementById('cardNumberInput')?.value.replace(/\D/g, '') || '';
        const cardExpiry = document.getElementById('cardExpiryInput')?.value || '';
        const cardCVV = document.getElementById('cardCvvInput')?.value || '';
        const cardholderName = document.getElementById('cardholderNameInput')?.value.trim() || '';

        // Card number
        if (!cardNumber || cardNumber.length !== 16) {
            alert('Please enter a valid 16-digit card number.');
            document.getElementById('cardNumberInput')?.focus();
            return false;
        }

        // Expiry date
        if (!cardExpiry) {
            alert('Please enter card expiry date.');
            document.getElementById('cardExpiryInput')?.focus();
            return false;
        }

        const expiryParts = cardExpiry.split('/');
        if (expiryParts.length !== 2 || expiryParts[0].length !== 2 || expiryParts[1].length !== 2) {
            alert('Please enter expiry date in MM/YY format.');
            document.getElementById('cardExpiryInput')?.focus();
            return false;
        }

        const month = parseInt(expiryParts[0]);
        const year = parseInt('20' + expiryParts[1]);
        
        if (month < 1 || month > 12) {
            alert('Please enter a valid month (01-12).');
            document.getElementById('cardExpiryInput')?.focus();
            return false;
        }

        // Check expiration
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear();
        const currentMonth = currentDate.getMonth() + 1;
        
        if (year < currentYear || (year === currentYear && month < currentMonth)) {
            alert('This card has expired. Please use a valid card.');
            document.getElementById('cardExpiryInput')?.focus();
            return false;
        }

        // CVV
        if (!cardCVV || cardCVV.length !== 3) {
            alert('Please enter a valid 3-digit CVV.');
            document.getElementById('cardCvvInput')?.focus();
            return false;
        }

        // Cardholder name
        if (!cardholderName || cardholderName.length < 2) {
            alert('Please enter a valid cardholder name.');
            document.getElementById('cardholderNameInput')?.focus();
            return false;
        }

        return true;
    }

    return false;
}

// Real-time validation indicators
function setupRealTimeValidation() {
    const gcashInput = document.getElementById('gcashNumberInput');
    const cardNumberInput = document.getElementById('cardNumberInput');
    const expiryInput = document.getElementById('cardExpiryInput');
    const cvvInput = document.getElementById('cardCvvInput');
    const nameInput = document.getElementById('cardholderNameInput');

    // GCash real-time validation
    if (gcashInput) {
        gcashInput.addEventListener('blur', function() {
            const value = this.value.replace(/\D/g, '');
            if (value && value.length === 11 && value.startsWith('09')) {
                this.classList.remove('border-red-500');
                this.classList.add('border-green-500');
            } else if (value) {
                this.classList.add('border-red-500');
                this.classList.remove('border-green-500');
            } else {
                this.classList.remove('border-red-500', 'border-green-500');
            }
        });
    }

    // Card number real-time validation
    if (cardNumberInput) {
        cardNumberInput.addEventListener('blur', function() {
            const value = this.value.replace(/\D/g, '');
            if (value && value.length === 16) {
                this.classList.remove('border-red-500');
                this.classList.add('border-green-500');
            } else if (value) {
                this.classList.add('border-red-500');
                this.classList.remove('border-green-500');
            } else {
                this.classList.remove('border-red-500', 'border-green-500');
            }
        });
    }

    // Expiry date real-time validation
    if (expiryInput) {
        expiryInput.addEventListener('blur', function() {
            const value = this.value;
            const expiryParts = value.split('/');
            if (expiryParts.length === 2 && expiryParts[0].length === 2 && expiryParts[1].length === 2) {
                const month = parseInt(expiryParts[0]);
                const year = parseInt('20' + expiryParts[1]);
                const currentDate = new Date();
                const currentYear = currentDate.getFullYear();
                const currentMonth = currentDate.getMonth() + 1;
                
                if (month >= 1 && month <= 12 && (year > currentYear || (year === currentYear && month >= currentMonth))) {
                    this.classList.remove('border-red-500');
                    this.classList.add('border-green-500');
                } else {
                    this.classList.add('border-red-500');
                    this.classList.remove('border-green-500');
                }
            } else if (value) {
                this.classList.add('border-red-500');
                this.classList.remove('border-green-500');
            } else {
                this.classList.remove('border-red-500', 'border-green-500');
            }
        });
    }

    // CVV real-time validation
    if (cvvInput) {
        cvvInput.addEventListener('blur', function() {
            const value = this.value;
            if (value && value.length === 3) {
                this.classList.remove('border-red-500');
                this.classList.add('border-green-500');
            } else if (value) {
                this.classList.add('border-red-500');
                this.classList.remove('border-green-500');
            } else {
                this.classList.remove('border-red-500', 'border-green-500');
            }
        });
    }

    // Cardholder name real-time validation
    if (nameInput) {
        nameInput.addEventListener('blur', function() {
            const value = this.value.trim();
            if (value && value.length >= 2) {
                this.classList.remove('border-red-500');
                this.classList.add('border-green-500');
            } else if (value) {
                this.classList.add('border-red-500');
                this.classList.remove('border-green-500');
            } else {
                this.classList.remove('border-red-500', 'border-green-500');
            }
        });
    }
}