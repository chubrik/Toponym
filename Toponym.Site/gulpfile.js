const cleanCss = require('gulp-clean-css');
const concat = require('gulp-concat');
const gulp = require('gulp');
const less = require('gulp-less');
const livereload = require('gulp-livereload');
const merge = require('merge2');
const stripCssComments = require('gulp-strip-css-comments');

gulp.task('less', () => {
    const normalizePath = gulp.src('node_modules/normalize.css/normalize.css');
    const bootstrapPath = gulp.src('node_modules/bootstrap/dist/css/bootstrap.css');
    const entryPath = gulp.src('app/less/app.less').pipe(less());
    const merged = merge(normalizePath, bootstrapPath, entryPath);

    const bundleDev = merged
        .pipe(concat('toponym.dev.css'));

    const bundleMin = merged
        .pipe(concat('toponym.min.css'))
        .pipe(stripCssComments({ preserve: false }))
        .pipe(cleanCss());

    return merge(bundleDev, bundleMin)
        .pipe(gulp.dest('wwwroot/css'))
        .pipe(livereload());
});

gulp.task('less:w', ['less'], () => {
    livereload.listen();
    gulp.watch('app/less/**/*.less', ['less']);
});
