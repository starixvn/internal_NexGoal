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

const MultiSelect = props => {
    if (props.value.length === props.options.length) {
        return (
            <ReactSelect className="react-select-container" classNamePrefix="react-select"
                {...props}
                isMulti hideSelectedOptions={false} isSearchable={false}
                components={{ Option, ValueContainer }}
                value={props.options}
                onChange={selected => props.onChange(selected)}
            />
        );
    }

    return (
        <ReactSelect className="react-select-container" classNamePrefix="react-select"
            {...props}
            isMulti hideSelectedOptions={false} isSearchable={false}
            components={{ Option, ValueContainer }}
            options={[props.allOption, ...props.options]}
            onChange={(selected) => {
                if (selected.length > 0 && selected[selected.length - 1].value === props.allOption.value) {
                    return props.onChange(props.options);
                }

                return props.onChange(selected);
            }}
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