import React from 'react';
import PropTypes from 'prop-types';
import { default as ReactSelect, components } from 'react-select';

const Option = (props) => (
    <components.Option {...props}>
        <input type="checkbox" checked={props.isSelected} onChange={(event) => { }} /> {props.data.label}
    </components.Option>
);

const ValueContainer = ({ children, ...props }) => {
    if (children[0] && children[0].length) {
        return (
            <components.ValueContainer {...props}>
                {children[0].length} item(s) selected
            </components.ValueContainer>
        );
    }

    return (
        <components.ValueContainer {...props}>
            {children}
        </components.ValueContainer>
    );
};

const MenuList = (props) => {
    var selected = props.getValue();

    return (
        <components.MenuList {...props}>
            <div style={{ padding: '8px 12px' }}>
                <input type="checkbox" checked={selected.length === props.options.length} onChange={(event) => {
                    if (event.target.checked) {
                        props.setValue(props.options);
                    } else {
                        props.setValue([]);
                    }
                }} /> Select All
            </div>
            {props.children}
        </components.MenuList >
    );
};

const MultiSelect = props => {
    return (
        <ReactSelect className="react-select-container" classNamePrefix="react-select"
            {...props}
            isMulti hideSelectedOptions={false} isSearchable={false}
            components={{ Option, ValueContainer, MenuList }}
        />
    );
};

MultiSelect.propTypes = {
    options: PropTypes.array,
    value: PropTypes.array,
    onChange: PropTypes.func,
    allOption: PropTypes.shape({
        label: PropTypes.string,
        value: PropTypes.string
    })
};

MultiSelect.defaultProps = {
    allOption: {
        value: '',
        label: 'Select all'
    }
};

export default MultiSelect;