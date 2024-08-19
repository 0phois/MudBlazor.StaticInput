let lastUrl = window.location.href;
let hasInitialized = false;

(function () {
    window.MutationObserverModule = {
        initialize: function () {
            function initializeScripts() {
                const currentUrl = window.location.href;

                if (hasInitialized && currentUrl === lastUrl) return;

                if (currentUrl !== lastUrl) {
                    lastUrl = currentUrl;

                    const hasStaticComponent = document.body.querySelector('[data-static-component]') !== null;

                    if (hasStaticComponent) {
                        window.location.reload();
                    }
                }
                hasInitialized = true;
            }
            const observer = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    if (mutation.type === 'childList' && window.location.href !== lastUrl) {
                        initializeScripts();
                        return false;
                    }
                });
            });
            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
            initializeScripts();
        }
    };

    function ensureInitialized() {
        if (window.MutationObserverModule && !hasInitialized) {
            window.MutationObserverModule.initialize();
        }
    }

    window.addEventListener('beforeunload', () => {
        hasInitialized = false;
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', ensureInitialized);
    } else {
        ensureInitialized();
    }

    if (window.Blazor) {
        window.Blazor.addEventListener('afterStarted', ensureInitialized);
    } else {
        document.addEventListener('DOMContentLoaded', () => {
            if (window.Blazor) {
                window.Blazor.addEventListener('afterStarted', ensureInitialized);
            }
        });
    }
})();