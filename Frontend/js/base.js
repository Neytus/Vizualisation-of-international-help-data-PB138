$(function () {
    data = {};
    loadData();
    $(window).resize(function () {
        resizeMap();
//        console.log($("#wrapper").height() + "; " + $("#map").width());
    });
});

/**
 * Load data from json. Initialize UI when data ready.
 */
function loadData() {
    $.getJSON("data/test.json", function (json) {
        data = json;
        generateCountryOptions();
        initUI();
        initMap();
    }).fail(function () {
        alert("Error. Data could not be loaded.");
    });
}

/**
 * Generate options for countries and years for selected country.
 */
function generateCountryOptions() {
    var i = 0;
    $.each(data, function (countryId, country) {
        // create country options
        var option = "<option value='" + countryId + "' ";
        if (i == 0) {
            option += "selected";
        }
        option += ">" + country.name + "</option>";
        $("#countrySelect").append(option);

        // update years based on country data, show data
        if (i == 0) {
            updateYearOptions(country);
            displaySum(country);
            displayAllData(country);
        }
        i++;
    });

}

/**
 * Update year options based on available data for given country
 * @param {Object} country - Object representing country with its data
 */
function updateYearOptions(country) {
    // clear year options
    $("#yearSelect").html("");
    // append "all" option to years select button
    var option = "<option value='All' selected>All</option>";
    $("#yearSelect").append(option);

    $.each(country.data, function (key, value) {
        if (key == "sum") {
            return;
        }
        // <option value="2017">2017</option>
        var option = "<option value='" + key + "'>" + key + "</option>";
        $("#yearSelect").append(option);
    });
}

/**
 * Initialize UI. Bind event handlers.
 */
function initUI() {
    $("#changeCountry").click(function () {
        var i = $("#selectForm #countrySelect").val();
        transition(i);
    });
    $("#countrySelect").select2({});
    $("#yearSelect").select2({});

    // update year options, display data for all years
    $("#countrySelect").on("change", function () {
        var selectedCountryId = $("#countrySelect").find(":selected").val();
        // update year options
        updateYearOptions(data[selectedCountryId]);
        // update map
        var i = $("#selectForm #countrySelect").val();
        transition(i);
        // display sum
        displaySum(data[selectedCountryId]);
        // display data for all years
        displayAllData(data[selectedCountryId]);
    });
    // display data for given year
    $("#yearSelect").on("change", function () {
        var selectedCountryId = $("#countrySelect").find(":selected").val();
        var selectedYearId = $("#yearSelect").find(":selected").val();
        displayYearData(data[selectedCountryId], selectedYearId);
    });
}

/**
 * 
 * @param {Object} country - Object representing Country with its data
 */
function displaySum(country) {
    var table = $("#dataTable");

    var undpt = "-";
    var undppc = "-";
    var wbt = "-";
    var wbpc = "-";

    if (typeof (country.data["sum"]["UNDP"]) != "undefined"
            && country.data["sum"]["UNDP"] !== null) {
        undpt = country.data["sum"]["UNDP"].budget;
        undppc = country.data["sum"]["UNDP"].budget_population;
    }
    if (typeof (country.data["sum"]["WorldBank"]) != "undefined"
            && country.data["sum"]["WorldBank"] !== null) {
        wbt = country.data["sum"]["WorldBank"].budget;
        wbpc = country.data["sum"]["WorldBank"].budget_population;
    }

    table.find(".total .undpt").html(undpt);
    table.find(".total .undppc").html(undppc);
    table.find(".total .wbt").html(wbt);
    table.find(".total .wbpc").html(wbpc);
}

/**
 * 
 * @param {type} country - Object representing Country with its data
 */
function displayAllData(country) {
    // delete displayed year rows
    $("#dataTable .yearRow").remove();

    // display all year rows
    $.each(country.data, function (year, value) {
        if (year === "sum") {
            return;
        }

        var undpt = "-";
        var undppc = "-";
        var wbt = "-";
        var wbpc = "-";

        if (typeof (country.data[year]["UNDP"]) != "undefined"
                && country.data[year]["UNDP"] !== null) {
            undpt = country.data[year]["UNDP"].budget;
            undppc = country.data[year]["UNDP"].budget_population;
        }
        if (typeof (country.data[year]["WorldBank"]) != "undefined"
                && country.data[year]["WorldBank"] !== null) {
            wbt = country.data[year]["WorldBank"].budget;
            wbpc = country.data[year]["WorldBank"].budget_population;
        }

        var row = '<tr class="yearRow">';
        row += '<td class="y">' + year + '</td>';
        row += '<td class="undpt">' + undpt + '</td>';
        row += '<td class="undppc">' + undppc + '</td>';
        row += '<td class="wbt">' + wbt + '</td>';
        row += '<td class="wbpc">' + wbpc + '</td>';
        row += '</tr>';
        $("#dataTable").append(row);
    });
}

function displayYearData(country, year) {
    // All option selected -> reroute to its function
    if (year == "All") {
        displayAllData(country);
        return;
    }

    // delete displayed year rows
    $("#dataTable .yearRow").remove();

    var undpt = "-";
    var undppc = "-";
    var wbt = "-";
    var wbpc = "-";

    if (typeof (country.data[year]["UNDP"]) != "undefined"
            && country.data[year]["UNDP"] !== null) {
        undpt = country.data[year]["UNDP"].budget;
        undppc = country.data[year]["UNDP"].budget_population;
    }
    if (typeof (country.data[year]["WorldBank"]) != "undefined"
            && country.data[year]["WorldBank"] !== null) {
        wbt = country.data[year]["WorldBank"].budget;
        wbpc = country.data[year]["WorldBank"].budget_population;
    }

    var row = '<tr class="yearRow">';
    row += '<td class="y">' + year + '</td>';
    row += '<td class="undpt">' + undpt + '</td>';
    row += '<td class="undppc">' + undppc + '</td>';
    row += '<td class="wbt">' + wbt + '</td>';
    row += '<td class="wbpc">' + wbpc + '</td>';
    row += '</tr>';
    $("#dataTable").append(row);
}

function resizeMap() {

}

function initMap() {
    map = {};
    if ($("#wrapper").height() < $("#map").width()) {
        map.width = $("#wrapper").height();
        map.height = $("#wrapper").height();
    } else {
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
    var country = map.countries.filter(function (obj) {
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