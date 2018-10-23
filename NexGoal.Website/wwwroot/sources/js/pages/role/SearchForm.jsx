import React, { Component } from 'react';
import { Form, ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';

class SearchForm extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.roleSelected !== nextProps.roleSelected
            || JSON.stringify(this.props.roleOptions) !== JSON.stringify(nextProps.roleOptions)) {
            return true;
        }
    }

    handleOnChange = (selectedOption) => {
        this.props.onRoleChange(selectedOption.value);
    }

    render() {
        const selectedOpt = this.props.roleSelected;
        const roleOptions = this.props.roleOptions.map((option) => {
            return { value: option.value, label: option.text };
        });
        const selectedOption = roleOptions.find((option) => { return option.value === selectedOpt.toString(); });

        return (
            <Form inline className="form-filter">
                <FormGroup>
                    <ControlLabel>{getResource('Role')}</ControlLabel>
                    <div className="control-content">
                        <Select className="react-select-container" classNamePrefix="react-select"
                            name="RoleId" isSearchable={false} isClearable={false}
                            value={selectedOption}
                            options={roleOptions}
                            onChange={this.handleOnChange}
                        />
                    </div>
                </FormGroup>
            </Form>
        );
    }
}

export default SearchForm;