$(function () {
    data = {};
    loadData();
});

/**
 * Load data from json. Initialize UI when data is ready.
 */
function loadData() {
    $.getJSON("data/finaljson.json", function (json) {
        data = json;
        generateCountryOptions();
        initUI();
        initMap();
    }).fail(function (msg) {
        console.log(msg);
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
 * Update year options based on available data for given country.
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
    // Submit button. Currently not used.
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
 * Display data contained under "sum" key ("Total" row in data table).
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
 * Display all available data for given country.
 * @param {Object} country - Object representing Country with its data
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

/**
 * Display data for selected country and year.
 * @param {Object} country - Object representing Country with its data
 * @param {String} year - Year to display data for
 */
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

/**
 * Initialize d3 map to display selected country.
 * Source: https://gist.github.com/tadast/8827699
 */
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

/**
 * Parse loaded data for map visualization.
 * @param {String} error - Error statement.
 * @param {Object} world - Object containing data to draw the world map.
 * @param {Array} names - Array of objects containing names and ids from "world-country-names.tsv".
 */
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

/**
 * Try to select and draw selected country on world map. Display message if data not found.
 * @param {String} i - Index of selected country to transition to.
 */
function transition(i) {
    var country = map.countries.filter(function (obj) {
        return obj.id == i;
    })[0];
    var selectedColor = "";
    var countryDisplayName = "";
    // no map data -> display previous country, greyed out
    if (country == "undefined" || country == null) {
        country = map.countries.filter(function (obj) {
            return obj.id == "4";
        })[0];
        selectedColor = "#ccc";
        countryDisplayName = $("#countrySelect option:selected").html();
        $("#mapDataInfo").show();
    }
    else {
        selectedColor = "#22BDF0";
        countryDisplayName = country.name;
        $("#mapDataInfo").hide();
    }
    mapTransition(country, selectedColor, countryDisplayName);
}

/**
 * Transition to selected country or default position if country map data not found.
 * @param {Object} country - Object containing data for selected country to display on the world map.
 * @param {String} selectedColor - Color to fill selected country area.
 * @param {String} countryDisplayName - Name of selected country to display.
 */
function mapTransition(country, selectedColor, countryDisplayName) {
    d3.transition()
            .duration(1250)
            .each("start", function () {
                countryName.text(countryDisplayName);
            })
            .tween("rotate", function () {
                var p = d3.geo.centroid(country),
                        r = d3.interpolate(map.projection.rotate(), [-p[0], -p[1]]);
                return function (t) {
                    map.projection.rotate(r(t));
                    c.clearRect(0, 0, map.width, map.height);
                    c.fillStyle = "#ccc", c.beginPath(), path(map.land), c.fill();
                    c.fillStyle = selectedColor, c.beginPath(), path(country), c.fill();
                    c.strokeStyle = "#fff", c.lineWidth = .5, c.beginPath(), path(map.borders), c.stroke();
                    c.strokeStyle = "#222", c.lineWidth = 1.3, c.beginPath(), path(map.globe), c.stroke();
                };
            });
}

