$(function () {
    initUI();
    initMap();
    $(window).resize(function () {
        resizeMap();
//        console.log($("#wrapper").height() + "; " + $("#map").width());
    });
});

function initUI() {
    $("#changeCountry").click(function () {
        var i = $("#selectForm #countrySelect").val();
        transition(i);
    });
    $("#countrySelect").select2({});
    $("#yearSelect").select2({});
}

function resizeMap() {
    
}

function initMap() {
    map = {};
    if ($("#wrapper").height() < $("#map").width()) {
        map.width = $("#wrapper").height();
        map.height = $("#wrapper").height();
    }
    else {
        map.width = $("#map").width();
        map.height = $("#map").width();
    }

    map.projection = d3.geo.orthographic()
            .translate([map.width / 2, map.height / 2])
            .scale(map.width / 2 - 20)
            .clipAngle(90)
            .precision(0.6);

    map.canvas = d3.select("#map").append("canvas")
            .attr("width", map.width)
            .attr("height", map.height);

    c = map.canvas.node().getContext("2d");

    path = d3.geo.path()
            .projection(map.projection)
            .context(c);

    countryName = d3.select("#countryName");

    queue().defer(d3.json, "./world-110m.json")
            .defer(d3.tsv, "./world-country-names.tsv")
            .await(ready);
}

function ready(error, world, names) {
    if (error)
        throw error;
    var i = 4;
    map.globe = {type: "Sphere"};
    map.land = topojson.feature(world, world.objects.land);
    map.countries = topojson.feature(world, world.objects.countries).features;
    map.borders = topojson.mesh(world, world.objects.countries, function (a, b) {
        return a !== b;
    });
    n = map.countries.length;

    map.countries = map.countries.filter(function (d) {
        return names.some(function (n) {
            if (d.id == n.id)
                return d.name = n.name;
        });
    });

    transition(i);
}

function transition(i) {
    var country = map.countries.filter(function(obj) {
        return obj.id == i;
    })[0];
    d3.transition()
            .duration(1250)
            .each("start", function () {
                countryName.text(country.name);
            })
            .tween("rotate", function () {
                var p = d3.geo.centroid(country),
                        r = d3.interpolate(map.projection.rotate(), [-p[0], -p[1]]);
                return function (t) {
                    map.projection.rotate(r(t));
                    c.clearRect(0, 0, map.width, map.height);
                    c.fillStyle = "#ccc", c.beginPath(), path(map.land), c.fill();
                    c.fillStyle = "#f00", c.beginPath(), path(country), c.fill();
                    c.strokeStyle = "#fff", c.lineWidth = .5, c.beginPath(), path(map.borders), c.stroke();
                    c.strokeStyle = "#000", c.lineWidth = 2, c.beginPath(), path(map.globe), c.stroke();
                };
            });
}