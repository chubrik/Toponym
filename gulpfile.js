// Paths
const path = require('path');
const repoRoot = __dirname;
const webRoot = path.join(repoRoot, './Toponym.Web');
const npmRoot = path.join(repoRoot, './node_modules');
const wwwRoot = path.join(webRoot, './wwwroot');

// Requires
const browserSync = require('browser-sync').create();
const cleanCss = require('gulp-clean-css');
const concat = require('gulp-concat');
const execSync = require('child_process').execSync;
const fs = require('fs-extra');
const gulp = require('gulp');
const less = require('gulp-less');
const merge = require('merge2');
const stripCssComments = require('gulp-strip-css-comments');

gulp.task('less', () => {
    const normalizePath = gulp.src(path.join(npmRoot, './normalize.css/normalize.css'));
    const bootstrapPath = gulp.src(path.join(npmRoot, './bootstrap/dist/css/bootstrap.css'));

    const entryPath =
        gulp.src(path.join(webRoot, './app/less/app.less'))
            .pipe(less({ paths: npmRoot, javascriptEnabled: true }));

    const merged = merge(normalizePath, bootstrapPath, entryPath);

    const bundleDev = merged
        .pipe(concat('toponym.dev.css'));

    const bundleMin = merged
        .pipe(concat('toponym.min.css'))
        .pipe(stripCssComments({ preserve: false }))
        .pipe(cleanCss());

    return merge(bundleDev, bundleMin)
        .pipe(gulp.dest(path.join(wwwRoot, './assets/css')));
});

gulp.task('watch', gulp.parallel(
    () => {
        gulp.watch(
            path.join(webRoot, './app/less/**').replace(/\\/g, '/'),
            gulp.series('less'));
    },
    () => {
        gulp.watch(
            path.join(webRoot, './$temp/js/**').replace(/\\/g, '/'),
            (done) => {
                execSync('node ./Toponym.Web/app/dev/ts-bundle.js');
                done();
            });
    },
    () => {
        browserSync.init({
            proxy: 'localhost:5000'
        });

        browserSync
            .watch([
                path.join(wwwRoot, './**/*'),
                path.join(webRoot, './Views/**')
            ])
            .on('change', browserSync.reload);
    }
));

gulp.task('clean', (done) => {
    fs.removeSync(path.join(repoRoot, './Toponym/bin'));
    fs.removeSync(path.join(repoRoot, './Toponym/obj'));
    fs.removeSync(path.join(repoRoot, './Toponym.Cli/bin'));
    fs.removeSync(path.join(repoRoot, './Toponym.Cli/obj'));
    fs.removeSync(path.join(webRoot, './bin'));
    fs.removeSync(path.join(webRoot, './obj'));
    fs.removeSync(path.join(webRoot, './$temp'));
    fs.removeSync(path.join(wwwRoot, './assets/css'));
    fs.removeSync(path.join(wwwRoot, './assets/js'));
    done();
});
