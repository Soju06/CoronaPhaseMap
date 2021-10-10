window.map = {
    setMap: (element) => {
        let beforePan = function (oldPan, newPan) {
            let stopHorizontal = false
                , stopVertical = false
                , gutterWidth = 500
                , gutterHeight = 500
                , sizes = this.getSizes()
                , leftLimit = -((sizes.viewBox.x + sizes.viewBox.width) * sizes.realZoom) + gutterWidth
                , rightLimit = sizes.width - gutterWidth - (sizes.viewBox.x * sizes.realZoom)
                , topLimit = -((sizes.viewBox.y + sizes.viewBox.height) * sizes.realZoom) + gutterHeight
                , bottomLimit = sizes.height - gutterHeight - (sizes.viewBox.y * sizes.realZoom)
            customPan = {}
            customPan.x = Math.max(leftLimit, Math.min(rightLimit, newPan.x))
            customPan.y = Math.max(topLimit, Math.min(bottomLimit, newPan.y))
            return customPan
        }
        let svg = svgPanZoom(element, {
            zoomEnabled: true,
            dblClickZoomEnabled: true,
            mouseWheelZoomEnabled: true,
            controlIconsEnabled: true,
            contain: true,
            center: true,
            minZoom: 0.8,
            maxZoom: 5,
            beforePan: beforePan,
            zoomScaleSensitivity: 0.5
        });
        new ResizeObserver(() => { svg.resize(); }).observe(element)
        return svg;
    }
}

window.toast = {
    init: (service) => {
        window.toastService = service;
    },
    toast: (message, time = 1000) => {
        let service = window.toastService;
        if (!service) return;
        service.invokeMethodAsync('Toast', message, time);
    }
};

const toast = window.toast.toast;

let polygons = []; // 테스트용

window.administrativeDistrict = {
    init: async (service, url, map) => {
        try {
            let res = await fetch(url);
            if (!res.ok) {
                toast(`행정구역을 가져오지 못했습니다. ${res.status}`, 2000);
                if (service) service.invokeMethodAsync('Error', res.status);
            }
            // 행정구역
            let data = await res.json();
            // 데이터 구역
            let bounds = data.bbox;

            // 맵을 데이터 구역으로 zoom.
            map.setBounds(latLngBounds(latLng(bounds[1], bounds[0]), latLng(bounds[3], bounds[2])));

            polygons = [];

            // 폴리곤 만들기
            const displayArea = (coordinates, name) => {
                // 폴리곤 패스
                let path = [];

                coordinates[0].forEach(c => path.push(latLng(c[1], c[0])));

                // 구역 폴리곤
                let polygon = mapPolygon({
                    map: map,
                    path: path,
                    strokeWeight: 3,
                    strokeColor: '#fff',
                    strokeOpacity: 0.4,
                    strokeStyle: 'solid',
                    fillColor: '#4088DA',
                    fillOpacity: 0.7,
                });

                console.log(name);
                polygons.push(polygon);

                kakao.maps.event.addListener(polygon, 'mouseover', function (mouseEvent) {
                    polygon.setOptions({ fillColor: '#132540' });
                });

                kakao.maps.event.addListener(polygon, 'mouseout', function () {
                    polygon.setOptions({ fillColor: '#4088DA' });
                });
            };

            // 행정지역 폴리곤 만들기
            data.features.forEach((val) => {
                let coordinates = val.geometry.coordinates;
                name = val.properties.SIG_KOR_NM;

                displayArea(coordinates, name);
            });
        } catch (err) {
            toast(`행정구역을 가져오지 못했습니다.\n${err}`, 2000);
        }
    },

};