const fs = require('fs-extra');
const minify = require('uglify-js').minify;
const path = require('path');
const rollup = require('rollup').rollup;
const rollupResolve = require('rollup-plugin-node-resolve');
const ts = require('typescript');

const projectRoot = path.join(__dirname, '../..');
const entryPath = path.join(projectRoot, './app/dist/js/app.module.js');
const outputDir = path.join(projectRoot, './wwwroot/js');
const modulesDir = path.join(projectRoot, './node_modules');

rollup({
    entry: entryPath,
    plugins: rollupResolve({
        browser: true,
        preferBuiltins: false
    })
})
    .then(bundle => {

        const rollupped = bundle.generate({
            format: 'iife',
            moduleName: 'Toponym'
        }).code;

        const compilerOptions = {
            allowJs: true,
            lib: ['es5', 'dom'],
            target: ts.ScriptTarget.ES5
        };

        const transpiled = ts.transpileModule(rollupped, { compilerOptions }).outputText;

        const jqueryDev = fs.readFileSync(path.join(modulesDir, './jquery/dist/jquery.js'), 'utf8');
        const angularDev = fs.readFileSync(path.join(modulesDir, './angular/angular.js'), 'utf8');
        const routerDev = fs.readFileSync(path.join(modulesDir, './angular-ui-router/release/angular-ui-router.js'), 'utf8');
        const bootstrapDev = fs.readFileSync(path.join(modulesDir, './angular-ui-bootstrap/dist/ui-bootstrap-tpls.js'), 'utf8');
        const mergedDev = [jqueryDev, angularDev, routerDev, bootstrapDev, transpiled].join('\r\n\r\n');
        fs.outputFileSync(path.join(outputDir, './toponym.dev.js'), mergedDev);

        const jqueryMin = fs.readFileSync(path.join(modulesDir, './jquery/dist/jquery.min.js'), 'utf8');
        const angularMin = fs.readFileSync(path.join(modulesDir, './angular/angular.min.js'), 'utf8');
        const routerMin = fs.readFileSync(path.join(modulesDir, './angular-ui-router/release/angular-ui-router.min.js'), 'utf8');
        const mergedMin = [jqueryMin, angularMin, routerMin, bootstrapDev, transpiled].join('\n');
        const minified = minify(mergedMin).code;
        fs.outputFileSync(path.join(outputDir, './toponym.min.js'), minified);
    })
    .catch(e => {
        console.error(e);
    });
