import React from 'react';
import { default as ReactSelect } from 'react-select';

import MultiValueContainer from './MultiValueContainer';
import MenuList from './MenuList';
import Option from './Option';

const MultiSelect = props => {
    return (
        <ReactSelect {...props}
            styles={{ multiValue: (base) => ({ ...base, backgroundColor: 'none', border: 'unset' }) }}
            isMulti
            hideSelectedOptions={false}
            closeMenuOnSelect={false}
            isSearchable={false}
            isClearable={false}
            menuPlacement="auto"
            //defaultMenuIsOpen={true}
            placeholder="0 item(s) selected"
            components={{ MultiValueContainer, MenuList, Option }}
        />
    );
};

export default MultiSelect;