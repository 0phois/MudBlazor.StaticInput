
console.log("MudBlazor.StaticInput JS initializer script loaded");

export function afterWebStarted(blazor) {
    console.log("MudBlazor.StaticInput afterWebStarted called");
    blazor.addEventListener('enhancedload', () => {
        initialize();
    });
    initialize();
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
    const textFields = document.querySelectorAll('[data-mud-static-type="text-field"]');
    textFields.forEach(inputElement => {
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
                inputElement.parentElement.classList.toggle("mud-shrink", !emptyValue);
            }
        }

        if (onAdornmentClick && onAdornmentClick.trim().length > 0) {
            const inputControl = inputElement.closest(".mud-input-control");
            const buttonElement = inputControl.querySelector('button');
            const icon = inputElement.getAttribute('data-mud-static-icon');
            const toggleIcon = inputElement.getAttribute('data-mud-static-toggle-icon');

            let isToggled = false;

            if (buttonElement) {
                // Remove existing listener if any (though enhancedload might replace the whole DOM)
                const newButton = buttonElement.cloneNode(true);
                buttonElement.parentNode.replaceChild(newButton, buttonElement);

                newButton.addEventListener('click', function (event) {
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
    const checkBoxes = document.querySelectorAll('[data-mud-static-type="checkbox"]');
    checkBoxes.forEach(checkbox => {
        const name = checkbox.getAttribute('data-mud-static-name');
        const parent = checkbox.closest('.mud-input-control');
        const emptyCheckbox = parent.querySelector('input[type="hidden"]');
        const checkedIcon = parent.querySelector('[id^="check-icon-"]');
        const uncheckedIcon = parent.querySelector('[id^="unchecked-icon-"]');

        checkbox.addEventListener('change', () => {
            checkbox.ariaChecked = checkbox.checked ? "true" : "false";
            if (checkedIcon) checkedIcon.style.display = checkbox.checked ? 'block' : 'none';
            if (uncheckedIcon) uncheckedIcon.style.display = checkbox.checked ? 'none' : 'block';

            if (checkbox.checked) {
                emptyCheckbox.removeAttribute("name");
                checkbox.setAttribute("name", name);
                checkbox.setAttribute("checked", "true");
            }
            else {
                checkbox.removeAttribute("name");
                checkbox.removeAttribute("checked");
                emptyCheckbox.setAttribute("name", name);
            }
        });
    });
}

function initSwitches() {
    const switches = document.querySelectorAll('[data-mud-static-type="switch"]');
    switches.forEach(switchToggle => {
        const name = switchToggle.getAttribute('data-mud-static-name');
        const parent = switchToggle.closest('.mud-input-control');
        const label = switchToggle.closest('label');
        const switchContainer = label.querySelector('[id^="switch-container-"]');
        const emptySwitch = label.querySelector('input[type="hidden"]');
        const switchTrack = label.querySelector('[id^="switch-track-"]');

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
                emptySwitch.removeAttribute("name");
                switchToggle.setAttribute("name", name);
                switchToggle.setAttribute("checked", "true");
            }
            else {
                switchToggle.removeAttribute("name");
                switchToggle.removeAttribute("checked");
                emptySwitch.setAttribute("name", name);
            }
        });
    });
}

function initRadios() {
    const radios = document.querySelectorAll('[data-mud-static-type="radio"]');
    radios.forEach(radio => {
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
                const checkedIcon = radioSpan.querySelector('[id^="radio-checked-icon-"]');
                const uncheckedIcon = radioSpan.querySelector('[id^="radio-unchecked-icon-"]');

                if (r.checked) {
                    if (checkedIcon) checkedIcon.style.display = 'block';
                    if (uncheckedIcon) uncheckedIcon.style.display = 'none';
                    r.setAttribute("checked", "");
                    r.setAttribute("aria-checked", "true");
                    r.setAttribute("name", groupName);
                    if (hiddenInput) {
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
    const drawerToggleElements = document.querySelectorAll('[data-mud-static-drawer-toggle]');

    drawerToggleElements.forEach(element => {
        element.addEventListener('click', (event) => {
            const targetDrawerId = event.currentTarget.getAttribute('data-mud-static-drawer-toggle');
            toggleDrawer(targetDrawerId);
        });
    });

    const responsiveDrawer = document.querySelector('.mud-drawer-responsive');
    if (responsiveDrawer) {
        monitorResize(responsiveDrawer);
    }
}

function toggleDrawer(drawerId) {
    let mudDrawer;
    if (!drawerId || drawerId === '_no_id_provided_') {
        mudDrawer = document.querySelector('.mud-drawer');
    } else {
        mudDrawer = document.getElementById(drawerId);
    }

    if (mudDrawer) {
        mudDrawer.classList.toggle('mud-drawer--open');
        mudDrawer.classList.toggle('mud-drawer--closed');
        mudDrawer.classList.remove('mud-drawer--initial');

        const header = document.querySelector('.mud-drawer-header');
        if (header) {
            if (mudDrawer.classList.contains('mud-drawer--closed')) {
                header.classList.add('mud-typography-nowrap');
            } else {
                header.classList.remove('mud-typography-nowrap');
            }
        }

        const layout = mudDrawer.parentElement;
        if (layout && layout.classList.contains('mud-layout')) {
            if (layout.className.includes('mud-drawer-open')) {
                layout.className = layout.className.replace(/\bmud-drawer-open\b/g, 'mud-drawer-close');
            } else {
                layout.className = layout.className.replace(/\bmud-drawer-close\b/g, 'mud-drawer-open');
                if (mudDrawer.classList.contains('mud-static-responsive')) {
                    mudDrawer.classList.add('mud-drawer-clipped-always');
                }
            }
        }
    }
}

function monitorResize(responsiveDrawer) {
    const classSections = Array.from(responsiveDrawer.parentElement.classList).find(className => className.includes('responsive')).split('-');
    const breakpoint = classSections[classSections.length - 2];
    const position = classSections[classSections.length - 1];
    const breakpointValue = getBreakpointValue(breakpoint);
    const resizeQuery = window.matchMedia(`(min-width: ${breakpointValue}px)`);

    const updateDrawer = (matches) => {
        if (matches) {
            autoExpand(responsiveDrawer, breakpoint, position);
        } else {
            autoCollapse(responsiveDrawer, breakpoint, position);
        }
    };

    updateDrawer(resizeQuery.matches);
    resizeQuery.addEventListener('change', ev => updateDrawer(ev.matches));
}

function getBreakpointValue(breakpoint) {
    switch (breakpoint) {
        case 'xs': return 380;
        case 'sm': return 600;
        case 'md': return 960;
        case 'lg': return 1280;
        case 'xl': return 1920;
        default: return 0;
    }
}

function autoCollapse(responsiveDrawer, breakpoint, position) {
    responsiveDrawer.classList.add('mud-drawer--closed');
    responsiveDrawer.classList.remove('mud-drawer--open');
    responsiveDrawer.parentElement.classList.add(`mud-drawer-close-responsive-${breakpoint}-${position}`);
    responsiveDrawer.parentElement.classList.remove(`mud-drawer-open-responsive-${breakpoint}-${position}`);
}

function autoExpand(responsiveDrawer, breakpoint, position) {
    responsiveDrawer.classList.add('mud-drawer--open');
    responsiveDrawer.classList.remove('mud-drawer--closed');
    responsiveDrawer.parentElement.classList.add(`mud-drawer-open-responsive-${breakpoint}-${position}`);
    responsiveDrawer.parentElement.classList.remove(`mud-drawer-close-responsive-${breakpoint}-${position}`);
}

function initNavGroups() {
    const navGroups = document.querySelectorAll('[data-mud-static-type="nav-group"]');
    navGroups.forEach(navGroup => {
        const button = navGroup.querySelector('button');
        if (button) {
            button.addEventListener('click', (event) => {
                const navElement = event.currentTarget.closest('.mud-nav-group');
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
