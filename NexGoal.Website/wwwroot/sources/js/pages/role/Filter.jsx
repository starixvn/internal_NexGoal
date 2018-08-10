import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

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
                        <ControlLabel>Role</ControlLabel>
                        <div className="control-content">
                            <Select
                                name="RoleId"
                                value={selectedOption}
                                options={options}
                                clearable={false}
                                searchable={false}
                                onChange={this.handleOnChange} />
                        </div>
                    </div>
                </FormGroup>
            </form>
        );
    }
}

export default Filter;