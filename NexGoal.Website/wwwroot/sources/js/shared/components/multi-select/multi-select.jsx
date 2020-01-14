import React, { PureComponent } from 'react';
import { default as ReactSelect } from 'react-select';

import MenuList from './MenuList';
import Option from './Option';

export default class MultiSelect extends PureComponent {
    render() {
        return (
            <ReactSelect {...this.props}
                isMulti
                hideSelectedOptions={false}
                closeMenuOnSelect={false}
                isClearable={false}
                menuPlacement="auto"
                backspaceRemovesValue={false}
                controlShouldRenderValue={false}
                placeholder={`${this.props.value.length} item(s) selected`}
                components={{ MenuList, Option }}
            />
        );
    }
}