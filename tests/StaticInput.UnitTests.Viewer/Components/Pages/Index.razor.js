let loadParams;

export function onLoad() {
    loadParams = new URLSearchParams(window.location.search);
}

export function onUpdate() {
    const updateParams = new URLSearchParams(window.location.search);

    if (updateParams.size == 0) {
        return;
    }

    if (updateParams.toString() !== loadParams.toString()) {
        window.location.reload();
    }
}

export function onDispose() {
    console.log('Home page dispose');
}