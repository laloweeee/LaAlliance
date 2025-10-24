// ==================== ORDER TRACKING MAP JS ====================
// Global variables
let isEditingAddress = false;
let usedCurrentLocation = false;
let mapInitialized = false;

// ==================== MAP INITIALIZATION ====================
document.addEventListener('DOMContentLoaded', function () {
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
    window.addEventListener('load', function () {
        if (typeof geocoder === 'undefined') {
            setTimeout(() => setupSearchButton(), 1000);
        }
    });

    // Initialize order type toggle
    initializeOrderTypeToggle();

    // Initialize address selection
    initializeAddressSelection();
});

// Check map initialization status
window.addEventListener('load', function () {
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
        radio.addEventListener('change', function () {
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

// ==================== FORM VALIDATION ====================
function validateFormBeforeSubmit() {
    // Validate payment method
    const paymentMethod = document.querySelector('input[name="PaymentMethod"]:checked');
    if (!paymentMethod) {
        alert('Please select a payment method.');
        return false;
    }

    // Validate delivery address for delivery orders
    const orderType = document.querySelector('input[name="OrderType"]:checked');
    if (orderType && orderType.value === 'Delivery') {
        const selectedAddress = document.querySelector('.address-radio:checked');

        if (!selectedAddress) {
            alert('Please select a delivery address for delivery orders.');
            return false;
        }
    }

    // All validations passed
    return true;
}