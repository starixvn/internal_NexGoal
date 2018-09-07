import React from 'react';
import { components } from 'react-select';

const Option = (props) => {
    return (
        <components.Option {...props}>
            <input type="checkbox" checked={props.isSelected} onChange={(event) => { }} /> {props.data.label}
        </components.Option>
    );
};

export default Option;