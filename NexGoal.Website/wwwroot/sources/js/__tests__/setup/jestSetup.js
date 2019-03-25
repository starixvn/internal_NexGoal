window._virtualConsole.removeAllListeners('jsdomError');
window.innerWidth = 1920;
window.innerHeight = 938;

document.getElementById = id => {
    let input;
    if (id === 'RootURL') {
        input = document.createElement('input', { value: '/' });
    } else {
        input = document.createElement('input', {
            value: 'test',
            innerText: 'test',
            attribute: () => 'image src'
        });
    }

    return input;
};

document.getElementByName = () => {
    const input1 = document.createElement('input', { value: '123' });
    const input2 = document.createElement('input', { value: '123' });

    return [input1, input2];
};