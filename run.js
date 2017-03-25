'use strict';

/* Modules */
const path = require('path');
const spawn = require('child_process').spawn;

/* Definitions */
const args = [
	'--use_strict', // Run app in strict mode
	'UnitatoBot.js'
];

const opt = {
	cwd: path.join(__dirname, 'src'),
	env: (function () {
		process.env.NODE_PATH = '.'; // Enables require() calls relative to the cwd :)
		return process.env;
	}()),
	stdio: [process.stdin, process.stdout, process.stderr]
};

/* Run */
spawn(process.execPath, args, opt);
