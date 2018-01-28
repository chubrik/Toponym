// Paths
const path = require('path');
const repoRoot = path.join(__dirname, '../..');
const siteRoot = path.join(repoRoot, './Toponym.Site');
const npmRoot = path.join(repoRoot, './node_modules');

// Requires
const fs = require('fs-extra');
const minify = require('uglify-js').minify;
const rollup = require('rollup').rollup;
const rollupResolve = require('rollup-plugin-node-resolve');
const ts = require('typescript');

const inputPath = path.join(siteRoot, './dist/js/app.module.js');
const outputDir = path.join(siteRoot, './wwwroot/js');

rollup({
    input: inputPath,
    plugins: rollupResolve({
        browser: true,
        preferBuiltins: false
    })
})
    .then(bundle => bundle.generate({
        format: 'iife',
        name: 'Toponym'
    }))
    .then(generated => {

        const compilerOptions = {
            allowJs: true,
            lib: ['es5', 'dom'],
            target: ts.ScriptTarget.ES5
        };

        const transpiled = ts.transpileModule(generated.code, { compilerOptions }).outputText;

        const jqueryDev = fs.readFileSync(path.join(npmRoot, './jquery/dist/jquery.js'), 'utf8');
        const angularDev = fs.readFileSync(path.join(npmRoot, './angular/angular.js'), 'utf8');
        const routerDev = fs.readFileSync(path.join(npmRoot, './@uirouter/angularjs/release/angular-ui-router.js'), 'utf8');
        const bootstrapDev = fs.readFileSync(path.join(npmRoot, './angular-ui-bootstrap/dist/ui-bootstrap-tpls.js'), 'utf8');
        const mergedDev = [jqueryDev, angularDev, routerDev, bootstrapDev, transpiled].join('\r\n\r\n');
        fs.outputFileSync(path.join(outputDir, './toponym.dev.js'), mergedDev);

        const jqueryMin = fs.readFileSync(path.join(npmRoot, './jquery/dist/jquery.min.js'), 'utf8');
        const angularMin = fs.readFileSync(path.join(npmRoot, './angular/angular.min.js'), 'utf8');
        const routerMin = fs.readFileSync(path.join(npmRoot, './@uirouter/angularjs/release/angular-ui-router.min.js'), 'utf8');
        const mergedMin = [jqueryMin, angularMin, routerMin, bootstrapDev, transpiled].join('\n');
        const minified = minify(mergedMin).code;
        fs.outputFileSync(path.join(outputDir, './toponym.min.js'), minified);
    })
    .catch(e => {
        console.error(e);
    });
