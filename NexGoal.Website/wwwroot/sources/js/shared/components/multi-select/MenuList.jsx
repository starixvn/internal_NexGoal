import React, { PureComponent } from 'react';
import { components } from 'react-select';

export default class MenuList extends PureComponent {
    render() {
        const selected = this.props.getValue();

        return (
            <components.MenuList {...this.props}>
                {
                    this.props.options.length > 0 &&
                    <div className="react-select__option"
                        onClick={() => {
                            if (selected.length === this.props.options.length) {
                                this.props.setValue([]);
                            } else {
                                this.props.setValue(this.props.options);
                            }
                        }}
                    >
                        <div className="checkbox">
                            <input className="checkbox-custom" type="checkbox" checked={selected.length === this.props.options.length}
                                onChange={() => {
                                    //empty
                                }}
                            /> <label>Select All</label>
                        </div>
                    </div>
                }
                {this.props.children}
            </components.MenuList>
        );
    }
}