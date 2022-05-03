import { has } from './utils';

let isFirstTime = true;

function checkFirstTime(): void {
    if (isFirstTime) {
        // tslint:disable-next-line:no-debugger
        debugger;
        isFirstTime = false;
    }
}

export function notImplemented(): Error {
    checkFirstTime();
    return new Error('Not implemented.');
}

export function outOfRange(argName: string): Error {
    checkArgument(argName, 'argName');
    checkFirstTime();
    return new Error(`Out of range '${argName}'.`);
}

export function invalidOperation(): Error {
    checkFirstTime();
    return new Error('Invalid operation.');
}

export function invalidArgument(argName: string): Error {
    checkFirstTime();
    return new Error(`Invalid argument ${argName || 'argName'}.`);
}

/** Допустимо: '', 0, false */
export function checkArgument(argValue: any, argName: string): void {

    if (!argName)
        throw invalidArgument('argName');

    if (!has(argValue))
        throw invalidArgument(argName);
}
