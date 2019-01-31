import React from 'react';
import { components } from 'react-select';

const MenuList = (props) => {
    let selected = props.getValue();

    return (
        <components.MenuList {...props}>
            {
                props.options.length > 0 &&
                <div className="react-select__option">
                    <input type="checkbox" checked={selected.length === props.options.length}
                        onChange={(event) => {
                            if (event.target.checked) {
                                props.setValue(props.options);
                            } else {
                                props.setValue([]);
                            }
                        }}
                    /> Select All
            </div>
            }
            {props.children}
        </components.MenuList >
    );
};

export default MenuList;