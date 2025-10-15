// Order Tracking Map Initialization
let map;
let markers = {};
let directionsService;
let directionsRenderer;

function initOrderTrackingMap() {
    const mapElement = document.getElementById('orderTrackingMap');
    if (!mapElement) return;

    // Get data from element attributes
    const deliveryLat = parseFloat(mapElement.dataset.deliveryLat);
    const deliveryLng = parseFloat(mapElement.dataset.deliveryLng);
    const deliveryAddress = mapElement.dataset.deliveryAddress;
    
    const restaurantLat = parseFloat(mapElement.dataset.restaurantLat);
    const restaurantLng = parseFloat(mapElement.dataset.restaurantLng);
    const restaurantAddress = mapElement.dataset.restaurantAddress;
    
    const driverLat = parseFloat(mapElement.dataset.driverLat);
    const driverLng = parseFloat(mapElement.dataset.driverLng);

    // Initialize map centered between restaurant and delivery location
    const centerLat = (deliveryLat + restaurantLat) / 2;
    const centerLng = (deliveryLng + restaurantLng) / 2;

    map = new google.maps.Map(mapElement, {
        zoom: 14,
        center: { lat: centerLat, lng: centerLng },
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: true,
        styles: [
            {
                featureType: "poi",
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            }
        ]
    });

    // Initialize directions service
    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({
        map: map,
        suppressMarkers: true,
        polylineOptions: {
            strokeColor: '#10B981',
            strokeWeight: 4,
            strokeOpacity: 0.8
        }
    });

    // Add Restaurant Marker
    if (restaurantLat && restaurantLng) {
        markers.restaurant = new google.maps.Marker({
            position: { lat: restaurantLat, lng: restaurantLng },
            map: map,
            title: 'Restaurant',
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 10,
                fillColor: '#EF4444',
                fillOpacity: 1,
                strokeColor: '#ffffff',
                strokeWeight: 2
            }
        });

        const restaurantInfoWindow = new google.maps.InfoWindow({
            content: `<div class="p-2">
                        <h3 class="font-semibold text-gray-800">Restaurant</h3>
                        <p class="text-sm text-gray-600">${restaurantAddress}</p>
                      </div>`
        });

        markers.restaurant.addListener('click', () => {
            restaurantInfoWindow.open(map, markers.restaurant);
        });
    }

    // Add Delivery Location Marker
    markers.delivery = new google.maps.Marker({
        position: { lat: deliveryLat, lng: deliveryLng },
        map: map,
        title: 'Delivery Location',
        icon: {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 10,
            fillColor: '#10B981',
            fillOpacity: 1,
            strokeColor: '#ffffff',
            strokeWeight: 2
        }
    });

    const deliveryInfoWindow = new google.maps.InfoWindow({
        content: `<div class="p-2">
                    <h3 class="font-semibold text-gray-800">Delivery Address</h3>
                    <p class="text-sm text-gray-600">${deliveryAddress}</p>
                  </div>`
    });

    markers.delivery.addListener('click', () => {
        deliveryInfoWindow.open(map, markers.delivery);
    });

    // Add Driver Marker (if available)
    if (driverLat && driverLng && driverLat !== 0 && driverLng !== 0) {
        markers.driver = new google.maps.Marker({
            position: { lat: driverLat, lng: driverLng },
            map: map,
            title: 'Driver',
            icon: {
                url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                    <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24">
                        <circle cx="12" cy="12" r="10" fill="#3B82F6"/>
                        <path d="M12 6 L16 18 L12 15 L8 18 Z" fill="white"/>
                    </svg>
                `),
                scaledSize: new google.maps.Size(32, 32),
                anchor: new google.maps.Point(16, 16)
            },
            animation: google.maps.Animation.BOUNCE
        });

        const driverInfoWindow = new google.maps.InfoWindow({
            content: `<div class="p-2">
                        <h3 class="font-semibold text-gray-800">Driver Location</h3>
                        <p class="text-sm text-gray-600">On the way to you</p>
                      </div>`
        });

        markers.driver.addListener('click', () => {
            driverInfoWindow.open(map, markers.driver);
        });

        // Draw route from driver to delivery location
        drawRoute(
            { lat: driverLat, lng: driverLng },
            { lat: deliveryLat, lng: deliveryLng }
        );
    } else if (restaurantLat && restaurantLng) {
        // If no driver location, show route from restaurant to delivery
        drawRoute(
            { lat: restaurantLat, lng: restaurantLng },
            { lat: deliveryLat, lng: deliveryLng }
        );
    }

    // Adjust map bounds to show all markers
    const bounds = new google.maps.LatLngBounds();
    if (restaurantLat && restaurantLng) {
        bounds.extend({ lat: restaurantLat, lng: restaurantLng });
    }
    bounds.extend({ lat: deliveryLat, lng: deliveryLng });
    if (driverLat && driverLng && driverLat !== 0 && driverLng !== 0) {
        bounds.extend({ lat: driverLat, lng: driverLng });
    }
    map.fitBounds(bounds);

    // Prevent over-zooming
    google.maps.event.addListenerOnce(map, 'bounds_changed', function() {
        if (map.getZoom() > 16) {
            map.setZoom(16);
        }
    });
}

function drawRoute(origin, destination) {
    const request = {
        origin: origin,
        destination: destination,
        travelMode: google.maps.TravelMode.DRIVING
    };

    directionsService.route(request, (result, status) => {
        if (status === google.maps.DirectionsStatus.OK) {
            directionsRenderer.setDirections(result);
        }
    });
}

// Update driver location (call this function periodically for real-time tracking)
function updateDriverLocation(lat, lng) {
    if (markers.driver) {
        const newPosition = { lat: lat, lng: lng };
        markers.driver.setPosition(newPosition);
        
        // Update route
        const deliveryPosition = markers.delivery.getPosition();
        drawRoute(newPosition, { 
            lat: deliveryPosition.lat(), 
            lng: deliveryPosition.lng() 
        });
    }
}

// Initialize map when page loads
document.addEventListener('DOMContentLoaded', initOrderTrackingMap);

