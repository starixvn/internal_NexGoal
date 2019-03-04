import React from 'react';
import { default as ReactSelect } from 'react-select';

import MenuList from './MenuList';
import Option from './Option';

const MultiSelect = props => {
    return (
        <ReactSelect {...props}
            styles={{ multiValue: (base) => ({ ...base, backgroundColor: 'none', border: 'unset' }) }}
            isMulti
            hideSelectedOptions={false}
            closeMenuOnSelect={false}
            isClearable={false}
            menuPlacement="auto"
            backspaceRemovesValue={false}
            controlShouldRenderValue={false}
            placeholder={`${props.value.length} item(s) selected`}
            components={{ MenuList, Option }}
        />
    );
};

export default MultiSelect;