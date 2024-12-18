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
            observer.observe(document.body, { childList: true, subtree: true });

            initializeScripts();
        }
    };

    window.MudDrawerInterop = {
        initialize: function () {
            const drawerToggleElements = document.querySelectorAll('[data-mud-drawer-toggle]');

            drawerToggleElements.forEach(element => {
                element.removeEventListener('click', this.handleToggleDrawer);
                element.addEventListener('click', this.handleToggleDrawer);
            });

            const responsiveDrawer = document.querySelectorAll('.mud-drawer-responsive')[0]; 

            if (responsiveDrawer) {
                MudDrawerInterop.monitorResize(responsiveDrawer);
            }
        },

        monitorResize(responsiveDrawer) {
            const classSections = Array.from(responsiveDrawer.parentElement.classList).find(className => className.includes('responsive')).split('-');
            const breakpoint = classSections[classSections.length - 2];
            const position = classSections[classSections.length - 1];
            const breakpointValue = MudDrawerInterop.getBreakpointValue(breakpoint);
            const resizeQuery = window.matchMedia(`(min-width: ${breakpointValue}px)`);

            if (responsiveDrawer.parentElement.offsetWidth <= breakpointValue) {
                MudDrawerInterop.autoCollapse(responsiveDrawer, breakpoint, position);
            } else {
                MudDrawerInterop.autoExpand(responsiveDrawer, breakpoint, position);
            }

            resizeQuery.addEventListener('change', ev => {
                if (ev.matches) {
                    MudDrawerInterop.autoExpand(responsiveDrawer, breakpoint, position);
                }
                else {
                    MudDrawerInterop.autoCollapse(responsiveDrawer, breakpoint, position);
                }
            })
        },

        getBreakpointValue(breakpoint) {
            switch (breakpoint) {
                case 'xs': return 380;
                case 'sm': return 600;
                case 'md': return 960;
                case 'lg': return 1280;
                case 'xl': return 1920;
            }
        },

        autoCollapse(responsiveDrawer, breakpoint, position) {
            responsiveDrawer.classList.add('mud-drawer--closed');
            responsiveDrawer.classList.remove('mud-drawer--open');
            responsiveDrawer.parentElement.classList.add(`mud-drawer-closed-responsive-${breakpoint}-${position}`)
            responsiveDrawer.parentElement.classList.remove(`mud-drawer-open-responsive-${breakpoint}-${position}`)
        },

        autoExpand(responsiveDrawer, breakpoint, position) {
            responsiveDrawer.classList.add('mud-drawer--open');
            responsiveDrawer.classList.remove('mud-drawer--closed');
            responsiveDrawer.parentElement.classList.add(`mud-drawer-open-responsive-${breakpoint}-${position}`)
            responsiveDrawer.parentElement.classList.remove(`mud-drawer-closed-responsive-${breakpoint}-${position}`)
        },

        handleToggleDrawer: function (event) {
            const element = event.currentTarget;
            const targetDrawerId = element.getAttribute('data-mud-drawer-toggle');

            MudDrawerInterop.toggleDrawer(targetDrawerId);
        },

        toggleDrawer: function (drawerId) {
            let mudDrawer;

            if (drawerId === '_no_id_provided_') {
                mudDrawer = document.querySelectorAll('.mud-drawer')[0];
            } else {
                mudDrawer = document.getElementById(drawerId);
            }

            if (mudDrawer) {
                mudDrawer.classList.toggle('mud-drawer--open');
                mudDrawer.classList.toggle('mud-drawer--closed');
                mudDrawer.classList.remove('mud-drawer--initial');
            }

            const header = document.querySelector('.mud-drawer-header');

            if (header) {
                if (mudDrawer.className.includes('mud-drawer--closed') === true) {
                    header.classList.add('mud-typography-nowrap');
                }
                else {
                    header.classList.remove('mud-typography-nowrap');
                }
            }

            const layout = mudDrawer.parentElement;

            if (layout) {
                if (layout.className.includes('mud-drawer-open') === true) {
                    layout.className = layout.className.replace(/\bmud-drawer-open\b/g, 'mud-drawer-closed');
                } else {
                    layout.className = layout.className.replace(/\bmud-drawer-closed\b/g, 'mud-drawer-open');

                    if (mudDrawer.className.includes('mud-static-responsive')) {
                        mudDrawer.classList.add('mud-drawer-clipped-always');
                    }
                }
            }
        },
    };

    function ensureInitialized() {
        if (window.MutationObserverModule && !hasInitialized) {
            window.MutationObserverModule.initialize();
            window.MudDrawerInterop.initialize();
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