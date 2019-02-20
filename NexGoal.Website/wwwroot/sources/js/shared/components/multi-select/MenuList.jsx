import React from 'react';
import { components } from 'react-select';

const MenuList = (props) => {
    let selected = props.getValue();

    return (
        <components.MenuList {...props}>
            {
                props.options.length > 0 &&
                <div className="react-select__option"
                    onClick={(event) => {
                        if (selected.length === props.options.length) {
                            props.setValue([]);
                        } else {
                            props.setValue(props.options);
                        }
                    }}
                >
                    <input type="checkbox" checked={selected.length === props.options.length}
                        onChange={(event) => { }}
                    /> Select All
                </div>
            }
            {props.children}
        </components.MenuList >
    );
};

export default MenuList;