module.exports = function (grunt) {
    grunt.initConfig({
        clean: ["wwwroot/lib/*"],
        uglify: {
            js: {
                src: ["wwwroot/js/site.js"],
                dest: "wwwroot/js/site.min.js"
            }
        },
        cssmin: {
            build: {
                src: ["wwwroot/css/site.css"],//压缩是要压缩合并了的
                dest: "wwwroot/css/site.min.css" //dest 是目的地输出
            }
        },
        copy: {
            client: {
                files: [
                    { expand: true, cwd: 'node_modules/bootstrap/dist/', src: '**', dest: "wwwroot/lib/bootstrap/" /*,filter: 'isFile' */ },
                    { expand: true, cwd: 'node_modules/jquery/dist/', src: '**', dest: "wwwroot/lib/jquery/" /*,filter: 'isFile' */ },
                    { expand: true, cwd: 'node_modules/jquery-validation/dist/', src: '**', dest: "wwwroot/lib/jquery-validation/" /*,filter: 'isFile' */ },
                    { expand: true, cwd: 'node_modules/', src: ['jquery-validation-unobtrusive/jquery.validate.unobtrusive.js*'], dest: "wwwroot/lib/" /*,filter: 'isFile' */ },
                    { expand: true, cwd: 'node_modules/live2d-widget/lib/', src: '**', dest: "wwwroot/lib/live2d-widget/" /*,filter: 'isFile' */ }
                ]
            }
        }
    });

    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-csslint');
    grunt.loadNpmTasks('grunt-contrib-copy');
};