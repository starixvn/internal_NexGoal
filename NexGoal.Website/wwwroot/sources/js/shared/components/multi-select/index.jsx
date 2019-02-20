import React from 'react';
import PropTypes from 'prop-types';
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
            defaultMenuIsOpen={true}
            components={{ MultiValueContainer, MenuList, Option }}
        />
    );
};

MultiSelect.propTypes = {
    options: PropTypes.arrayOf(PropTypes.object),
    value: PropTypes.arrayOf(PropTypes.object),
    onChange: PropTypes.func
};

export default MultiSelect;