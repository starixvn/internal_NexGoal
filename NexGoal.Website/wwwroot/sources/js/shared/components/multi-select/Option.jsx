import React from 'react';
import { components } from 'react-select';

const Option = (props) => {
    return (
        <components.Option {...props}>
          <div className="checkbox">
            <input className="checkbox-custom" type="checkbox" checked={props.isSelected} onChange={(event) => { }} />
            <label>{props.data.label}</label>
          </div>
        </components.Option>
    );
};

export default Option;