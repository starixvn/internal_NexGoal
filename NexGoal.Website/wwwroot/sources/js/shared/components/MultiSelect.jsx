import React from 'react';
import { default as ReactSelect } from "react-select";

const MultiSelect = props => {
    let valueComponent = (option) => {
        let length = 0;

        if (props.simpleValue) {
            length = props.value.split(',').length;
        } else {
            length = props.value.length;
        }
        return (<span>{length} selected</span >);
    }

    let options = { ...{ valueComponent, multi: true }, ...props };

    return <ReactSelect {...options} />;
};

export default MultiSelect;