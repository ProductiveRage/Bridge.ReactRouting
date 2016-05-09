/// <binding AfterBuild='RunUnitTests' />
var gulp = require("gulp"),
    notifier = require('node-notifier'),
    chutzpah = require("gulp-chutzpah");

var opts = {
    executable: "..\\packages\\Chutzpah.4.2.1\\tools\\chutzpah.console.exe",
    parallelism: true, // All tests should be isolated so might as well run as many in parallel as possible (see https://www.npmjs.com/package/gulp-chutzpah#options)
    silent: true, // Don't show "Tests complete: x" as they are run
    nologo: true
};

gulp.task("RunUnitTests", function () {
    gulp
        .src("./Bridge/output/ProductiveRage.ReactRouting.Tests.js")
        .pipe(chutzpah(opts))
        .on('error', function (e) {
            var d = new Date();
            var formattedTime = (d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds()).replace(/(^|:)(\d)(?=:|\.)/g, '$10$2'); // Courtesy of http://stackoverflow.com/a/21979752
            notifier.notify({
                title: 'ReactRouting Tests (' + formattedTime + ')',
                message: 'At least one of the unit tests has failed, please check Tests.html in the browser',
                sound: true
            });
        });
});