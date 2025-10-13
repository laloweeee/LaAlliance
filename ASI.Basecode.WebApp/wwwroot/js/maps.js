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
    if (marker) {
        marker.setPosition(location);
        marker.setAnimation(google.maps.Animation.BOUNCE);
        setTimeout(() => marker.setAnimation(null), 750);
    } else {
        marker = new google.maps.Marker({
            position: location,
            map: map,
            draggable: true,
            animation: google.maps.Animation.DROP,
            title: 'Drag to adjust location'
        });

        marker.addListener('dragend', function (event) {
            updateCoordinates(event.latLng.lat(), event.latLng.lng(), latInputId, lngInputId);
        });
    }

    updateCoordinates(location.lat(), location.lng(), latInputId, lngInputId);
    map.panTo(location);
}

/**
 * Update hidden inputs and display with new coordinates
 * @param {number} lat - Latitude
 * @param {number} lng - Longitude
 * @param {string} latInputId - The ID of the latitude hidden input
 * @param {string} lngInputId - The ID of the longitude hidden input
 */
function updateCoordinates(lat, lng, latInputId = 'latitude', lngInputId = 'longitude') {
    document.getElementById(latInputId).value = lat;
    document.getElementById(lngInputId).value = lng;
    updateCoordinatesDisplay(lat, lng);
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
                console.error('Geolocation error:', error);
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
    if (currentLocBtn) {
        currentLocBtn.addEventListener('click', function () {
            useCurrentLocation();
        });
    }
}

/**
 * Initialize all map functionality on page load
 */
function initMaps() {
    // Initialize address picker map if element exists
    if (document.getElementById('addressMap')) {
        initAddressMap();
        setupSearchButton();
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