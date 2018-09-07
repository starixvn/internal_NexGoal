import React from 'react';
import { components } from 'react-select';

const MenuList = (props) => {
    let selected = props.getValue();

    return (
        <components.MenuList {...props}>
            <div style={{ padding: '8px 12px' }}>
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
            {props.children}
        </components.MenuList >
    );
};

export default MenuList;