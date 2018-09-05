import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.handleOnChange = this.handleOnChange.bind(this);
    }

    handleOnChange(selectedOption) {
        this.props.onRoleChange(selectedOption.value);
    }

    render() {
        const selectedOpt = this.props.selectedOption;
        const options = this.props.options.map((option) => {
            return { value: option.value, label: option.text };
        });
        const selectedOption = options.find((option) => { return option.value === selectedOpt.toString(); });

        return (
            <form className="form-inline form-filter">
                <FormGroup>
                    <div className="col">
                        <ControlLabel>{getResource('Role')}</ControlLabel>
                        <div className="control-content">
                            <Select className="react-select-container" classNamePrefix="react-select"
                                name="RoleId" isSearchable={false} isClearable={false}
                                value={selectedOption}
                                options={options}
                                onChange={this.handleOnChange}
                            />
                        </div>
                    </div>
                </FormGroup>
            </form>
        );
    }
}

export default Filter;