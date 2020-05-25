var plotToMap = plotToMap || (function () {
    var _args = {}; // private
    var _mapType = "CQI";
    var _data = {};
    var _mapElementPrefix = 'map-';
    function getPlotColor(data) {
        var _iColor = "orange";
        if (_mapType == "LTE_UE_CQI_CQI") {
            if (parseInt(data.LTE_UE_CQI_CQI) == 6 || parseInt(data.LTE_UE_CQI_CQI) == 7) {
            }
            else if (parseInt(data.LTE_UE_CQI_CQI) >= 8 && parseInt(data.LTE_UE_CQI_CQI) <= 10) {
                _iColor = "ylw"
            }
            else if (parseInt(data.LTE_UE_CQI_CQI) >= 11 && parseInt(data.LTE_UE_CQI_CQI) <= 12) {
                _iColor = "grn";
            }
        } else if (_mapType == "LTE_UE_RSSNR_RSSNR") {
            if (this.parseInt(data.LTE_UE_RSSNR_RSSNR) >= 6 && this.parseInt(data.LTE_UE_RSSNR_RSSNR) <= 9) {
            }
            else if (this.parseInt(data.LTE_UE_RSSNR_RSSNR) >= 10  && this.parseInt(data.LTE_UE_RSSNR_RSSNR) <= 13) {
                _iColor = "ylw"
            }
            else if (this.parseInt(data.LTE_UE_RSSNR_RSSNR) >= 14 && this.parseInt(data.LTE_UE_RSSNR_RSSNR) <= 17) {
                _iColor = "grn";
            }
            else {
                _iColor = "red";
            }
        } else if (_mapType == "LTE_UE_RSRP_RSRP") {
            if (this.parseInt(data.LTE_UE_RSRP_RSRP) >= -80) {
                _iColor = "grn";
            }
            else if (this.parseInt(data.LTE_UE_RSRP_RSRP) >= -90 && this.parseInt(data.LTE_UE_RSRP_RSRP) < -80) {
                _iColor = "ylw"
            }
            else if (this.parseInt(data.LTE_UE_RSRP_RSRP) >= -100 && this.parseInt(data.LTE_UE_RSRP_RSRP) < -91) {
                _iColor = "orange";
            }
            else if (this.parseInt(data.LTE_UE_RSRP_RSRP < -100)) {
                _iColor = 'red';
            }
        } else if (_mapType == "LTE_UE_RSRQ_RSRQ") {
            if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) >= 10) {
                _iColor = "grn";
            }
            else if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) >= -15 && this.parseInt(data.LTE_UE_RSRQ_RSRQ) < -10) {
                _iColor = "ylw"
            }
            else if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) >= -20 && this.parseInt(data.LTE_UE_RSRQ_RSRQ) < -15) {
                _iColor = "orange";
            }
            else if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) < -20){
                _iColor = "red";
            }
        } else if (_mapType == "LTE_UE_RSRQ_RSRQ") {
            if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) >= 10) {
                _iColor = "grn";
            }
            else if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) >= -15 && this.parseInt(data.LTE_UE_RSRQ_RSRQ) < -10) {
                _iColor = "ylw"
            }
            else if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) >= -20 && this.parseInt(data.LTE_UE_RSRQ_RSRQ) < -15) {
                _iColor = "orange";
            }
            else if (this.parseInt(data.LTE_UE_RSRQ_RSRQ) < -20) {
                _iColor = "red";
            }
        } else if (_mapType == "LTE_UE_SERVER_RAT") {
            _iColor = "blu";
        } else if (_mapType == "LTE_UE_RI_RI") {
            _iColor = "grn";
        } else if (_mapType == "LTE_UE_SERVER_BAND" || _mapType == "LTE_UE_SERVER_CARRIER" || _mapType == "LTE_UE_SERVER_FREQUENCY" || _mapType == "LTE_UE_SERVER_PCI" ) {
            _iColor = "ylw";
        }

        return _iColor;
    }
    function getMarkertTitle(data) {
        var title = data.LTE_UE_CQI_CQI;
        if (_mapType == "CQI") {
            title = data.LTE_UE_CQI_CQI;
        } else if (_mapType == "RSRP") {
            title = data.LTE_UE_RSRP_RSRP;
        } else if (_mapType == "RSSNR") {
            title = data.LTE_UE_RSSNR_RSSNR;
        }
        return title;
    }
    return {
        init: function (mapType, data) {
            // _mapType = mapType;
            ShowOverlay();
            _data = data;
            for (var i = 0; i < mapType.length; i++) {
                var map = document.createElement("div")
                map.id = _mapElementPrefix + (i + 1);
                map.classList.add('mapStyle');
                //map.classList.add('col-md-12');
                document.getElementById("map_view").appendChild(map);
                _mapType = mapType[i];
                this.loadMap(map.id);
            }
            HideOverlay();
        },
        loadMap: function (mapElementId) {
            var mapOptions = {
                center: new google.maps.LatLng(_data[1].MapPoints.LTE_UE_GPS_latitude, _data[1].MapPoints.LTE_UE_GPS_longitude), //which city will be shown.
                zoom: 20, //google map page zoom
                mapTypeId: google.maps.MapTypeId.ROADMAP //type of view.
            };
            var infoWindow = new google.maps.InfoWindow();
            var map = new google.maps.Map(document.getElementById(mapElementId), mapOptions);//pass div id and google map load values.
            for (i = 0; i < _data.length; i++) { //here load all city map name
                var data = _data[i].MapPoints;
                var color = getPlotColor(data);
                var image = {
                    url: 'http://54.39.176.251/Images/' + color + '-blank-lv.png',
                    // This marker is 20 pixels wide by 32 pixels high.
                    size: new google.maps.Size(20, 20),
                    // The origin for this image is (0, 0).
                    origin: new google.maps.Point(0, 0),
                    // The anchor for this image is the base of the flagpole at (0, 32).
                    anchor: new google.maps.Point(0, 32)
                };

                var myLatlng = new google.maps.LatLng(data.LTE_UE_GPS_latitude, data.LTE_UE_GPS_longitude); //here i assigned lat and long.
                var marker = new google.maps.Marker({
                    position: myLatlng, //lat and long value
                    map: map, //div id
                    title: data[_mapType],
                    icon: image//'http://maps.google.com/mapfiles/ms/icons/green-dot.png'
                    //marker.setIcon()
                });
                (function (marker, data) { //here city map description after click on it.
                    google.maps.event.addListener(marker, "click", function (e) {
                        infoWindow.setContent("testing"); //city description value
                        infoWindow.open(map, marker); //then pop will show  the description
                    });
                })(marker, data);
            }

        }
    };
}());