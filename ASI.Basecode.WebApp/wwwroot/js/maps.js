let map;
let marker;
let geocoder;

/**
 * Initialize the address map for location picker
 * @param {string} mapElementId - The ID of the map container element
 * @param {string} latInputId - The ID of the latitude hidden input
 * @param {string} lngInputId - The ID of the longitude hidden input
 */
function initAddressMap(mapElementId = 'addressMap', latInputId = 'latitude', lngInputId = 'longitude') {
    const latInput = document.getElementById(latInputId);
    const lngInput = document.getElementById(lngInputId);

    const existingLat = parseFloat(latInput.value) || 14.5995;
    const existingLng = parseFloat(lngInput.value) || 120.9842;

    geocoder = new google.maps.Geocoder();

    const initialLocation = { lat: existingLat, lng: existingLng };

    map = new google.maps.Map(document.getElementById(mapElementId), {
        zoom: 15,
        center: initialLocation,
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: true,
        zoomControl: true,
        rotateControl: false,
        scaleControl: false,
        gestureHandling: 'greedy',
        styles: [
            {
                featureType: "poi",
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            },
            {
                featureType: "poi",
                elementType: "geometry",
                stylers: [{ visibility: "off" }]
            }
        ]
    });

    // Create initial marker if coordinates exist
    if (latInput.value && lngInput.value) {
        marker = new google.maps.Marker({
            position: initialLocation,
            map: map,
            draggable: true,
            animation: google.maps.Animation.DROP,
            title: 'Drag to adjust location'
        });

        updateCoordinatesDisplay(existingLat, existingLng);

        // Update coordinates when marker is dragged
        marker.addListener('dragend', function (event) {
            updateCoordinates(event.latLng.lat(), event.latLng.lng(), latInputId, lngInputId);
        });
    }

    // Click on map to set/move location
    map.addListener('click', function (event) {
        placeMarker(event.latLng, latInputId, lngInputId);
    });
}

/**
 * Place or move marker on the map
 * @param {google.maps.LatLng} location - The location to place the marker
 * @param {string} latInputId - The ID of the latitude hidden input
 * @param {string} lngInputId - The ID of the longitude hidden input
 */
function placeMarker(location, latInputId = 'latitude', lngInputId = 'longitude') {
    console.log('📌 placeMarker called with:', location.lat(), location.lng());
    
    if (marker) {
        console.log('↔️ Moving existing marker...');
        marker.setPosition(location);
        marker.setAnimation(google.maps.Animation.BOUNCE);
        setTimeout(() => marker.setAnimation(null), 750);
    } else {
        console.log('🆕 Creating new marker...');
        marker = new google.maps.Marker({
            position: location,
            map: map,
            draggable: true,
            animation: google.maps.Animation.DROP,
            title: 'Drag to adjust location'
        });

        marker.addListener('dragend', function (event) {
            console.log('🎯 Marker dragged to new position');
            updateCoordinates(event.latLng.lat(), event.latLng.lng(), latInputId, lngInputId);
        });
    }

    console.log('🔄 About to call updateCoordinates...');
    updateCoordinates(location.lat(), location.lng(), latInputId, lngInputId);
    map.panTo(location);
    console.log('✅ placeMarker completed');
}

/**
 * Update hidden inputs and display with new coordinates
 * @param {number} lat - Latitude
 * @param {number} lng - Longitude
 * @param {string} latInputId - The ID of the latitude hidden input
 * @param {string} lngInputId - The ID of the longitude hidden input
 */
function updateCoordinates(lat, lng, latInputId = 'latitude', lngInputId = 'longitude') {
    console.log('📍 updateCoordinates called:', { lat, lng, latInputId, lngInputId });
    
    document.getElementById(latInputId).value = lat;
    document.getElementById(lngInputId).value = lng;
    updateCoordinatesDisplay(lat, lng);
    
    // Perform reverse geocoding to auto-fill address fields
    reverseGeocode(lat, lng);
}

/**
 * Reverse geocode coordinates to extract address components
 * @param {number} lat - Latitude
 * @param {number} lng - Longitude
 */
function reverseGeocode(lat, lng) {
    console.log('🔄 reverseGeocode called with:', lat, lng);
    
    if (!geocoder) {
        console.error('❌ Geocoder not initialized!');
        return;
    }
    
    console.log('✅ Geocoder is ready, starting reverse geocoding...');
    
    const latlng = { lat: lat, lng: lng };
    
    geocoder.geocode({ location: latlng }, (results, status) => {
        console.log('📡 Geocoder response - Status:', status);
        
        if (status === 'OK') {
            if (results[0]) {
                console.log('✅ Got geocoding results:', results);
                fillAddressFields(results[0]);
            } else {
                console.warn('⚠️ No results returned');
            }
        } else {
            console.error('❌ Reverse geocoding failed:', status);
        }
    });
}

/**
 * Fill address form fields from geocoded result
 * @param {google.maps.GeocoderResult} result - Geocoder result
 */
function fillAddressFields(result) {
    console.log('🗺️ Reverse geocoding result:', result);
    console.log('📋 Full formatted address:', result.formatted_address);
    
    const addressComponents = result.address_components;
    console.log('📦 Address components:', addressComponents);
    
    // Extract components
    let street = '';
    let barangay = '';
    let city = '';
    let province = '';
    let zipCode = '';
    
    // Parse address components
    for (let component of addressComponents) {
        const types = component.types;
        
        if (types.includes('street_number')) {
            street = component.long_name + ' ';
        }
        if (types.includes('route')) {
            street += component.long_name;
        }
        if (types.includes('sublocality') || types.includes('sublocality_level_1')) {
            barangay = component.long_name;
        }
        if (types.includes('locality') || types.includes('administrative_area_level_2')) {
            city = component.long_name;
        }
        if (types.includes('administrative_area_level_1')) {
            province = component.long_name;
        }
        if (types.includes('postal_code')) {
            zipCode = component.long_name;
        }
    }
    
    // Fallback: if barangay not found, try sublocality_level_2 or neighborhood
    if (!barangay) {
        for (let component of addressComponents) {
            const types = component.types;
            if (types.includes('sublocality_level_2') || types.includes('neighborhood')) {
                barangay = component.long_name;
                break;
            }
        }
    }
    
    // If still no street found, use formatted address first part
    if (!street.trim()) {
        const formattedParts = result.formatted_address.split(',');
        if (formattedParts.length > 0) {
            street = formattedParts[0].trim();
        }
    }
    
    console.log('📍 Extracted address components:', { street, barangay, city, province, zipCode });
    
    // Try multiple ways to find form fields
    const streetInput = document.querySelector('input[name="Street"]') || 
                       document.querySelector('input[asp-for="Street"]') ||
                       document.getElementById('Street');
    
    const barangayInput = document.querySelector('input[name="Barangay"]') || 
                         document.querySelector('input[asp-for="Barangay"]') ||
                         document.getElementById('Barangay');
    
    const cityInput = document.querySelector('input[name="City"]') || 
                     document.querySelector('input[asp-for="City"]') ||
                     document.getElementById('City');
    
    const provinceInput = document.querySelector('input[name="Province"]') || 
                         document.querySelector('input[asp-for="Province"]') ||
                         document.getElementById('Province');
    
    const zipCodeInput = document.querySelector('input[name="ZipCode"]') || 
                        document.querySelector('input[asp-for="ZipCode"]') ||
                        document.getElementById('ZipCode');
    
    console.log('🔍 Found inputs:', {
        streetInput: !!streetInput,
        barangayInput: !!barangayInput,
        cityInput: !!cityInput,
        provinceInput: !!provinceInput,
        zipCodeInput: !!zipCodeInput
    });
    
    let fieldsUpdated = 0;
    
    // Update Street
    if (streetInput && street) {
        console.log('⏩ Updating Street field with:', street.trim());
        streetInput.value = street.trim();
        streetInput.classList.add('bg-green-50', 'border-green-300');
        setTimeout(() => {
            streetInput.classList.remove('bg-green-50', 'border-green-300');
        }, 2000);
        fieldsUpdated++;
        console.log('✅ Street updated:', street.trim());
    } else {
        console.log('⚠️ Street not updated - Input:', !!streetInput, 'Value:', street);
    }
    
    // Update Barangay
    if (barangayInput && barangay) {
        console.log('⏩ Updating Barangay field with:', barangay);
        barangayInput.value = barangay;
        barangayInput.classList.add('bg-green-50', 'border-green-300');
        setTimeout(() => {
            barangayInput.classList.remove('bg-green-50', 'border-green-300');
        }, 2000);
        fieldsUpdated++;
        console.log('✅ Barangay updated:', barangay);
    } else {
        console.log('⚠️ Barangay not updated - Input:', !!barangayInput, 'Value:', barangay);
    }
    
    // Update City
    if (cityInput && city) {
        console.log('⏩ Updating City field with:', city);
        cityInput.value = city;
        cityInput.classList.add('bg-green-50', 'border-green-300');
        setTimeout(() => {
            cityInput.classList.remove('bg-green-50', 'border-green-300');
        }, 2000);
        fieldsUpdated++;
        console.log('✅ City updated:', city);
    } else {
        console.log('⚠️ City not updated - Input:', !!cityInput, 'Value:', city);
    }
    
    // Update Province
    if (provinceInput && province) {
        console.log('⏩ Updating Province field with:', province);
        provinceInput.value = province;
        provinceInput.classList.add('bg-green-50', 'border-green-300');
        setTimeout(() => {
            provinceInput.classList.remove('bg-green-50', 'border-green-300');
        }, 2000);
        fieldsUpdated++;
        console.log('✅ Province updated:', province);
    } else {
        console.log('⚠️ Province not updated - Input:', !!provinceInput, 'Value:', province);
    }
    
    // Update Zip Code
    if (zipCodeInput && zipCode) {
        console.log('⏩ Updating ZipCode field with:', zipCode);
        zipCodeInput.value = zipCode;
        zipCodeInput.classList.add('bg-green-50', 'border-green-300');
        setTimeout(() => {
            zipCodeInput.classList.remove('bg-green-50', 'border-green-300');
        }, 2000);
        fieldsUpdated++;
        console.log('✅ Zip Code updated:', zipCode);
    } else {
        console.log('⚠️ ZipCode not updated - Input:', !!zipCodeInput, 'Value:', zipCode);
    }
    
    console.log(`✨ Auto-filled ${fieldsUpdated} address field(s)`);
}

/**
 * Update the coordinates display on the page
 * @param {number} lat - Latitude
 * @param {number} lng - Longitude
 */
function updateCoordinatesDisplay(lat, lng) {
    const displayLat = document.getElementById('displayLat');
    const displayLng = document.getElementById('displayLng');

    if (displayLat && displayLng) {
        displayLat.textContent = lat.toFixed(6);
        displayLng.textContent = lng.toFixed(6);
    }
}

/**
 * Initialize a display-only map (non-interactive)
 * @param {string} mapElementId - The ID of the map container element
 * @param {number} lat - Latitude
 * @param {number} lng - Longitude
 * @param {string} title - Marker title
 * @param {string} infoContent - HTML content for info window
 */
function initDisplayMap(mapElementId, lat, lng, title = '', infoContent = '') {
    const location = { lat: lat, lng: lng };

    const displayMap = new google.maps.Map(document.getElementById(mapElementId), {
        zoom: 16,
        center: location,
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        zoomControl: true,
        rotateControl: false,
        scaleControl: false,
        gestureHandling: 'greedy',
        styles: [
            {
                featureType: "poi",
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            },
            {
                featureType: "poi",
                elementType: "geometry",
                stylers: [{ visibility: "off" }]
            }
        ]
    });

    const displayMarker = new google.maps.Marker({
        position: location,
        map: displayMap,
        animation: google.maps.Animation.DROP,
        title: title
    });

    if (infoContent) {
        const infoWindow = new google.maps.InfoWindow({
            content: infoContent
        });

        displayMarker.addListener('click', () => {
            infoWindow.open(displayMap, displayMarker);
        });
    }
}

/**
 * Get current location using browser's geolocation API
 * @param {string} latInputId - The ID of the latitude hidden input
 * @param {string} lngInputId - The ID of the longitude hidden input
 */
function useCurrentLocation(latInputId = 'latitude', lngInputId = 'longitude') {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            function (position) {
                const location = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                map.setCenter(location);
                map.setZoom(17);
                placeMarker(new google.maps.LatLng(location.lat, location.lng), latInputId, lngInputId);
            },
            function (error) {
                alert('Unable to get your current location. Please set location manually.');
            }
        );
    } else {
        alert('Geolocation is not supported by your browser.');
    }
}

/**
 * Setup use current location button event listener
 * @param {string} buttonId - The ID of the current location button
 */
function setupCurrentLocationButton(buttonId = 'currentLocationBtn') {
    const currentLocBtn = document.getElementById(buttonId);
    
    if (!currentLocBtn) {
        console.error('❌ Current location button not found! ID:', buttonId);
        return;
    }
    
    
    currentLocBtn.addEventListener('click', function () {
        
        // Show loading state
        const originalHTML = this.innerHTML;
        this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Getting Location...';
        this.disabled = true;
        
        // Store reference to button for callback
        const btn = this;
        
        if (navigator.geolocation) {
            console.log('📡 Requesting geolocation...');
            
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    
                    const location = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };
                    
                    map.setCenter(location);
                    map.setZoom(17);
                    placeMarker(new google.maps.LatLng(location.lat, location.lng));
                    
                    // Show success state
                    btn.innerHTML = '<i class="fas fa-check"></i> Location Set!';
                    setTimeout(() => {
                        btn.innerHTML = originalHTML;
                        btn.disabled = false;
                    }, 2000);
                },
                function (error) {
                    console.error('Geolocation error:', error);
                    alert('Unable to get your current location. Please set location manually.');
                    btn.innerHTML = originalHTML;
                    btn.disabled = false;
                }
            );
        } else {
            alert('Geolocation is not supported by your browser.');
            btn.innerHTML = originalHTML;
            btn.disabled = false;
        }
    });
}

/**
 * Initialize all map functionality on page load
 */
function initMaps() {
    // Initialize address picker map if element exists
    if (document.getElementById('addressMap')) {
        initAddressMap();
        // setupSearchButton is defined in the page script section
        if (typeof setupSearchButton === 'function') {
            setupSearchButton();
        }
        setupCurrentLocationButton();
    }

    // Initialize display map if element exists
    const displayMapElement = document.getElementById('map');
    if (displayMapElement) {
        const lat = parseFloat(displayMapElement.dataset.lat);
        const lng = parseFloat(displayMapElement.dataset.lng);
        const title = displayMapElement.dataset.title || '';
        const address = displayMapElement.dataset.address || '';

        const infoContent = `<div class="p-2">
                                <h3 class="font-bold text-lg">${title}</h3>
                                <p class="text-sm text-gray-600">${address}</p>
                             </div>`;

        initDisplayMap('map', lat, lng, title, infoContent);
    }
}

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initMaps);
} else {
    initMaps();
}