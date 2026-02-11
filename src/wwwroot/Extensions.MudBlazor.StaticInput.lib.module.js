
export function afterWebStarted(blazor) {
    if (blazor) {
        blazor.addEventListener('enhancedload', () => {
            initialize();
        });
    }

    initialize();

    // Use MutationObserver to catch elements added dynamically (e.g. during WASM transition or dynamic rendering)
    const observer = new MutationObserver((mutations) => {
        let shouldInit = false;
        for (const mutation of mutations) {
            if (mutation.addedNodes.length > 0) {
                shouldInit = true;
                break;
            }
        }
        if (shouldInit) {
            initialize();
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

function initialize() {
    initTextFields();
    initCheckBoxes();
    initSwitches();
    initDrawers();
    initNavGroups();
    initRadios();
}

function initTextFields() {
    const textFields = document.querySelectorAll('[data-mud-static-type="text-field"]:not([data-mud-static-initialized="true"])');
    textFields.forEach(inputElement => {
        inputElement.setAttribute('data-mud-static-initialized', 'true');
        const shrinkLabel = inputElement.getAttribute('data-mud-static-shrink') === 'true';
        const showOnFocus = inputElement.getAttribute('data-mud-static-helper-focus') === 'true';
        const onAdornmentClick = inputElement.getAttribute('data-mud-static-adornment-click');

        const parentElement = inputElement.closest(".mud-input-control");
        const helperElement = parentElement ? parentElement.querySelector(".me-auto") : null;

        if (!shrinkLabel || showOnFocus) {
            if (showOnFocus && helperElement) {
                helperElement.style.visibility = "hidden";

                inputElement.addEventListener('focus', function () {
                    helperElement.style.visibility = "visible";
                });
            }

            inputElement.addEventListener('blur', function (event) {
                if (!shrinkLabel) {
                    const emptyValue = event.target.value.length === 0;
                    const targetParent = event.target.parentElement;

                    targetParent.classList.toggle("mud-shrink", !emptyValue);
                }

                if (showOnFocus && helperElement) {
                    helperElement.style.visibility = "hidden";
                }
            });

            // Initial state for shrink
            if (!shrinkLabel) {
                const emptyValue = inputElement.value.length === 0;
                const targetParent = inputElement.parentElement;
                if (targetParent) {
                    targetParent.classList.toggle("mud-shrink", !emptyValue);
                }
            }
        }

        if (onAdornmentClick && onAdornmentClick.trim().length > 0) {
            const inputControl = inputElement.closest(".mud-input-control");
            const buttonElement = inputControl ? inputControl.querySelector('button') : null;
            const icon = inputElement.getAttribute('data-mud-static-icon');
            const toggleIcon = inputElement.getAttribute('data-mud-static-toggle-icon');

            let isToggled = false;

            if (buttonElement) {
                buttonElement.addEventListener('click', function (event) {
                    const currentSvg = event.currentTarget.querySelector("svg");
                    if (currentSvg && toggleIcon) {
                        isToggled = !isToggled;
                        currentSvg.innerHTML = isToggled ? toggleIcon : icon;
                    }

                    if (typeof window[onAdornmentClick] === 'function') {
                        window[onAdornmentClick](inputElement, event.currentTarget);
                    }
                });
            }
        }
    });
}

function initCheckBoxes() {
    const checkBoxes = document.querySelectorAll('[data-mud-static-type="checkbox"]:not([data-mud-static-initialized="true"])');
    checkBoxes.forEach(checkbox => {
        checkbox.setAttribute('data-mud-static-initialized', 'true');
        const name = checkbox.getAttribute('data-mud-static-name');
        const parent = checkbox.closest('.mud-input-control');
        const emptyCheckbox = parent ? parent.querySelector('input[type="hidden"]') : null;
        const checkedIcon = parent ? parent.querySelector('[id^="check-icon-"]') : null;
        const uncheckedIcon = parent ? parent.querySelector('[id^="unchecked-icon-"]') : null;

        checkbox.addEventListener('change', () => {
            checkbox.ariaChecked = checkbox.checked ? "true" : "false";
            if (checkedIcon) checkedIcon.style.display = checkbox.checked ? 'block' : 'none';
            if (uncheckedIcon) uncheckedIcon.style.display = checkbox.checked ? 'none' : 'block';

            if (checkbox.checked) {
                if (emptyCheckbox) emptyCheckbox.removeAttribute("name");
                checkbox.setAttribute("name", name);
                checkbox.setAttribute("checked", "");
            }
            else {
                checkbox.removeAttribute("name");
                checkbox.removeAttribute("checked");
                if (emptyCheckbox) emptyCheckbox.setAttribute("name", name);
            }
        });
    });
}

function initSwitches() {
    const switches = document.querySelectorAll('[data-mud-static-type="switch"]:not([data-mud-static-initialized="true"])');
    switches.forEach(switchToggle => {
        switchToggle.setAttribute('data-mud-static-initialized', 'true');
        const name = switchToggle.getAttribute('data-mud-static-name');
        const label = switchToggle.closest('label');
        const switchContainer = label ? label.querySelector('[id^="switch-container-"]') : null;
        const emptySwitch = label ? label.querySelector('input[type="hidden"]') : null;
        const switchTrack = label ? label.querySelector('[id^="switch-track-"]') : null;

        const checkedTextColor = switchToggle.getAttribute('data-mud-static-checked-text-color');
        const checkedHoverColor = switchToggle.getAttribute('data-mud-static-checked-hover-color');
        const uncheckedTextColor = switchToggle.getAttribute('data-mud-static-unchecked-text-color');
        const uncheckedHoverColor = switchToggle.getAttribute('data-mud-static-unchecked-hover-color');
        const checkedColor = switchToggle.getAttribute('data-mud-static-checked-color');
        const uncheckedColor = switchToggle.getAttribute('data-mud-static-unchecked-color');

        switchToggle.addEventListener('change', () => {
            switchToggle.ariaChecked = switchToggle.checked ? "true" : "false";
            const force = switchToggle.checked;

            if (switchContainer) {
                switchContainer.classList.toggle("mud-checked", force);
                if (checkedTextColor) switchContainer.classList.toggle(checkedTextColor, force);
                if (checkedHoverColor) switchContainer.classList.toggle(checkedHoverColor, force);
                if (uncheckedTextColor) switchContainer.classList.toggle(uncheckedTextColor, !force);
                if (uncheckedHoverColor) switchContainer.classList.toggle(uncheckedHoverColor, !force);
            }

            if (switchTrack) {
                if (checkedColor) switchTrack.classList.toggle(checkedColor, force);
                if (uncheckedColor) switchTrack.classList.toggle(uncheckedColor, !force);
            }

            if (switchToggle.checked) {
                if (emptySwitch) emptySwitch.removeAttribute("name");
                switchToggle.setAttribute("name", name);
                switchToggle.setAttribute("checked", "");
            }
            else {
                switchToggle.removeAttribute("name");
                switchToggle.removeAttribute("checked");
                if (emptySwitch) emptySwitch.setAttribute("name", name);
            }
        });
    });
}

function initRadios() {
    const radios = document.querySelectorAll('[data-mud-static-type="radio"]:not([data-mud-static-initialized="true"])');
    radios.forEach(radio => {
        radio.setAttribute('data-mud-static-initialized', 'true');
        radio.addEventListener('change', function () {
            const parentGroup = radio.closest('[data-mud-static-type="radio-group"]');
            if (!parentGroup) return;

            const hiddenInput = parentGroup.querySelector("input[type='hidden']");
            const selectedValue = radio.getAttribute('data-value');
            const groupName = radio.getAttribute('data-group-name');

            parentGroup.querySelectorAll('[data-mud-static-type="radio"]').forEach(function (r) {
                if (r !== radio) {
                    r.checked = false;
                    r.removeAttribute("name");
                    r.removeAttribute("checked");
                    r.setAttribute("aria-checked", "false");
                }

                const radioSpan = r.closest('span');
                const checkedIcon = radioSpan ? radioSpan.querySelector('[id^="radio-checked-icon-"]') : null;
                const uncheckedIcon = radioSpan ? radioSpan.querySelector('[id^="radio-unchecked-icon-"]') : null;

                if (r.checked) {
                    if (checkedIcon) checkedIcon.style.display = 'block';
                    if (uncheckedIcon) uncheckedIcon.style.display = 'none';
                    r.setAttribute("checked", "");
                    r.setAttribute("aria-checked", "true");
                    if (groupName) r.setAttribute("name", groupName);
                    if (hiddenInput && selectedValue !== null) {
                        hiddenInput.value = selectedValue;
                        hiddenInput.setAttribute("value", selectedValue);
                    }
                } else {
                    if (checkedIcon) checkedIcon.style.display = 'none';
                    if (uncheckedIcon) uncheckedIcon.style.display = 'block';
                }
            });
        });
    });
}

function initDrawers() {
    const drawerToggleElements = document.querySelectorAll('[data-mud-static-type="drawer-toggle"]:not([data-mud-static-initialized="true"])');
    drawerToggleElements.forEach(element => {
        element.setAttribute('data-mud-static-initialized', 'true');
        element.removeEventListener('click', onDrawerToggleClick);
        element.addEventListener('click', onDrawerToggleClick);
    });

    const drawers = document.querySelectorAll('.mud-drawer-responsive, .mud-drawer-mini, .mud-drawer-persistent');
    drawers.forEach(drawer => {
        monitorResize(drawer);
    });
}

function onDrawerToggleClick(event) {
    const targetDrawerId =
        event.currentTarget.getAttribute('data-mud-static-drawer-toggle');

    toggleDrawer(targetDrawerId);
}

function getStorageKey(mudDrawer, drawerId) {
    // If a specific drawerId was provided by the toggle, use it.
    if (drawerId && drawerId !== '_no_id_provided_') {
        return `mud-static-drawer-open-${drawerId}`;
    }

    // If we have a drawer element and it has an ID, check if it was specifically set.
    // MudBlazor generates IDs like '_mud_drawer_...' if not provided.
    if (mudDrawer && mudDrawer.id && mudDrawer.id.length > 0 && !mudDrawer.id.startsWith('_mud_')) {
        return `mud-static-drawer-open-${mudDrawer.id}`;
    }

    // Fallback to default
    return 'mud-static-drawer-open-default';
}

function updateStorage(key, value) {
    localStorage.setItem(key, value);
    // Set a cookie that expires in 1 year
    const cookieString = `${key}=${value}; path=/; max-age=31536000; SameSite=Lax`;
    document.cookie = cookieString;
}

function getStoredState(key) {
    const ls = localStorage.getItem(key);
    if (ls !== null) {
        return ls === 'true';
    }

    const match = document.cookie.match(new RegExp('(^| )' + key + '=([^;]+)'));
    if (match) {
        return match[2] === 'true';
    }

    return null;
}

function toggleDrawer(drawerId) {
    let mudDrawer;
    if (!drawerId || drawerId === '_no_id_provided_') {
        mudDrawer = document.querySelector('.mud-drawer');
    } else {
        mudDrawer = document.getElementById(drawerId);
    }

    if (mudDrawer) {
        const wasOpen = mudDrawer.classList.contains('mud-drawer--open');
        const isNowOpen = !wasOpen;

        applyDrawerState(mudDrawer, isNowOpen);

        const storageKey = getStorageKey(mudDrawer, drawerId);
        updateStorage(storageKey, isNowOpen);
    }
}

function applyDrawerState(mudDrawer, isOpen) {
    if (isOpen) {
        mudDrawer.classList.add('mud-drawer--open');
        mudDrawer.classList.remove('mud-drawer--closed');
    } else {
        mudDrawer.classList.add('mud-drawer--closed');
        mudDrawer.classList.remove('mud-drawer--open');
    }

    mudDrawer.classList.remove('mud-drawer--initial');

    const header = mudDrawer.querySelector('.mud-drawer-header');
    if (header) {
        if (!isOpen) {
            header.classList.add('mud-typography-nowrap');
        } else {
            header.classList.remove('mud-typography-nowrap');
        }
    }

    const layout = mudDrawer.parentElement;
    if (layout) {
        // Identify the anchor (left or right) of the drawer to avoid affecting other drawers
        const anchor = mudDrawer.classList.contains('mud-drawer-pos-right') ? 'right' : 'left';

        // Try to replace the specific responsive/mini layout class that matches this drawer's anchor
        const classList = Array.from(layout.classList);
        const layoutClass = classList.find(c =>
            (c.startsWith('mud-drawer-open-') || c.startsWith('mud-drawer-close-')) &&
            c.endsWith(`-${anchor}`)
        );

        if (layoutClass) {
            const newClass = isOpen
                ? layoutClass.replace('mud-drawer-close-', 'mud-drawer-open-')
                : layoutClass.replace('mud-drawer-open-', 'mud-drawer-close-');

            if (newClass !== layoutClass) {
                layout.classList.remove(layoutClass);
                layout.classList.add(newClass);
            }
        } else {
            // Fallback for non-responsive drawers if they use simple classes.
            // Note: MudBlazor usually uses anchor-specific classes even for simple ones,
            // but we keep this as a generic fallback.
            if (!isOpen) {
                layout.classList.remove('mud-drawer-open');
                layout.classList.add('mud-drawer-close');
            } else {
                layout.classList.remove('mud-drawer-close');
                layout.classList.add('mud-drawer-open');
            }
        }

        if (isOpen && mudDrawer.classList.contains('mud-static-responsive')) {
            mudDrawer.classList.add('mud-drawer-clipped-always');
        }
    }
}

function setDrawerState(drawerId, isOpen) {
    let mudDrawer;
    if (!drawerId || drawerId === '_no_id_provided_') {
        mudDrawer = document.querySelector('.mud-drawer');
    } else {
        mudDrawer = document.getElementById(drawerId);
    }

    const storageKey = getStorageKey(mudDrawer, drawerId);
    updateStorage(storageKey, isOpen);

    if (mudDrawer) {
        applyDrawerState(mudDrawer, isOpen);
    }
}

window.MudDrawerInterop = {
    toggleDrawer: toggleDrawer,
    setDrawerState: setDrawerState,
    getDrawerState: function(drawerId) {
        return getStoredState(getStorageKey(null, drawerId));
    }
};

function monitorResize(mudDrawer) {
    if (mudDrawer.hasAttribute('data-mud-static-monitored')) return;
    mudDrawer.setAttribute('data-mud-static-monitored', 'true');


    const parentElement = mudDrawer.parentElement;
    if (!parentElement) return;

    // Identify the anchor (left or right) of the drawer
    const anchor = mudDrawer.classList.contains('mud-drawer-pos-right') ? 'right' : 'left';

    // Try to find breakpoint and position from layout classes matching this drawer's anchor
    // Expected formats: mud-drawer-[open/close]-[responsive/mini/persistent]-[breakpoint]-[position]
    const layoutClass = Array.from(parentElement.classList).find(className =>
        className.startsWith('mud-drawer-') &&
        (className.includes('-responsive-') || className.includes('-mini-') || className.includes('-persistent-')) &&
        className.endsWith(`-${anchor}`)
    );

    if (!layoutClass) {
        // If we can't find the layout class on the parent, we can still apply the stored state
        const storageKey = getStorageKey(mudDrawer);
        const storedState = getStoredState(storageKey);
        if (storedState !== null) {
            applyDrawerState(mudDrawer, storedState);
        }
        return;
    }

    const classSections = layoutClass.split('-');
    let variant, breakpoint, position;

    // mud, drawer, state, variant, [breakpoint], position
    if (classSections.length === 6) {
        variant = classSections[3];
        breakpoint = classSections[4];
        position = classSections[5];
    } else if (classSections.length === 5) {
        variant = classSections[3];
        breakpoint = 'none';
        position = classSections[4];
    } else {
        return;
    }

    if (breakpoint === 'none') {
        // Not a responsive drawer, just apply stored state if it exists
        const storageKey = getStorageKey(mudDrawer);
        const storedState = localStorage.getItem(storageKey);
        if (storedState !== null) {
            applyDrawerState(mudDrawer, storedState === 'true');
        }
        return;
    }

    const breakpointValue = getBreakpointValue(breakpoint);
    const resizeQuery = window.matchMedia(`(min-width: ${breakpointValue}px)`);

    const updateDrawer = (matches) => {
        const storageKey = getStorageKey(mudDrawer);
        const storedState = getStoredState(storageKey);

        if (storedState !== null) {
            applyDrawerState(mudDrawer, storedState);
        } else {
            // No stored state, follow breakpoint
            if (matches) {
                autoExpand(mudDrawer, variant, breakpoint, position);
            } else {
                autoCollapse(mudDrawer, variant, breakpoint, position);
            }
        }
    };

    updateDrawer(resizeQuery.matches);
    resizeQuery.addEventListener('change', ev => {
        // When screen size changes, we might want to override the stored state if it was a responsive toggle?
        // Actually, MudBlazor's behavior is complex here.
        // For now, let's just re-evaluate. If we want resize to ALWAYS override, we should clear localStorage on resize.
        // But usually we want to stay open/closed if the user clicked it.
        updateDrawer(ev.matches);
    });
}

function getBreakpointValue(breakpoint) {
    switch (breakpoint) {
        case 'xs': return 0;
        case 'sm': return 600;
        case 'md': return 960;
        case 'lg': return 1280;
        case 'xl': return 1920;
        case 'xxl': return 2560;
        default: return 0;
    }
}

function autoCollapse(mudDrawer, variant, breakpoint, position) {
    applyDrawerState(mudDrawer, false);
    if (mudDrawer.parentElement) {
        mudDrawer.parentElement.classList.add(`mud-drawer-close-${variant}-${breakpoint}-${position}`);
        mudDrawer.parentElement.classList.remove(`mud-drawer-open-${variant}-${breakpoint}-${position}`);
    }
}

function autoExpand(mudDrawer, variant, breakpoint, position) {
    applyDrawerState(mudDrawer, true);
    if (mudDrawer.parentElement) {
        mudDrawer.parentElement.classList.add(`mud-drawer-open-${variant}-${breakpoint}-${position}`);
        mudDrawer.parentElement.classList.remove(`mud-drawer-close-${variant}-${breakpoint}-${position}`);
    }
}

function initNavGroups() {
    const navGroups = document.querySelectorAll('[data-mud-static-type="nav-group"]:not([data-mud-static-initialized="true"])');
    navGroups.forEach(navGroup => {
        navGroup.setAttribute('data-mud-static-initialized', 'true');
        const button = navGroup.querySelector('button');
        if (button) {
            button.addEventListener('click', (event) => {
                const navElement = event.currentTarget.closest('.mud-nav-group');
                if (!navElement) return;
                const collapseContainer = navElement.querySelector('.mud-collapse-container');
                const expandIcon = navElement.querySelector('.mud-nav-link-expand-icon');

                if (!collapseContainer || !expandIcon) return;

                const isExpanded = button.getAttribute('aria-expanded') === "true";

                collapseContainer.classList.toggle('mud-collapse-entered', !isExpanded);
                collapseContainer.classList.toggle('mud-navgroup-collapse', true);
                collapseContainer.classList.remove('mud-collapse-entering');
                collapseContainer.setAttribute('aria-hidden', isExpanded);

                expandIcon.classList.toggle('mud-transform', !isExpanded);
                button.setAttribute('aria-expanded', !isExpanded);
            });
        }
    });
}
