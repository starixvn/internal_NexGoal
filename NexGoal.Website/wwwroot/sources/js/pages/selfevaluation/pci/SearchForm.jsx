import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Select from 'react-select';

import MultiSelect from '../../../shared/components/multi-select/index';
import getResource from '../../../resourceHelper';

class SearchForm extends Component {
    constructor(props) {
        super(props);

        this.onFromDateChange = this.onFromDateChange.bind(this);
        this.onToDateChange = this.onToDateChange.bind(this);
        this.onChangeRaw = this.onChangeRaw.bind(this);
        this.handleCreatorChange = this.handleCreatorChange.bind(this);
        this.handleProjectChange = this.handleProjectChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    onFromDateChange(date) {
        this.props.onFilterChange('fromDate', date);
    }

    onToDateChange(date) {
        this.props.onFilterChange('toDate', date);
    }

    onChangeRaw(event) {
        let date = moment(event.target.value);
        if (date.isValid()) {
            this.props.onFilterChange(event.target.name, date);
        }
    }

    handleCreatorChange(selectedOptions) {
        this.props.onFilterChange('creators', selectedOptions);
    }

    handleProjectChange(selectedOptions) {
        this.props.onFilterChange('projects', selectedOptions);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.onFilterSubmit();
    }

    render() {
        let filter = this.props.filter;
        let creatorOptions = this.props.creatorOptions;
        let projectOptions = this.props.projectOptions;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <div className="row-filter">
                    <FormGroup>
                        <ControlLabel>{getResource('FromDate')}</ControlLabel>
                        <DatePicker dateFormat="DD MMM, YYYY" selected={filter.fromDate}
                            onChange={this.onFromDateChange} onChangeRaw={this.onChangeRaw}
                            name="fromDate" className="form-control" autoComplete="off"
                        />
                    </FormGroup>
                    <FormGroup>
                        <ControlLabel>{getResource('ToDate')}</ControlLabel>
                        <DatePicker dateFormat="DD MMM, YYYY" selected={filter.toDate}
                            onChange={this.onToDateChange} onChangeRaw={this.onChangeRaw}
                            name="toDate" className="form-control" autoComplete="off"
                        />
                    </FormGroup>
                    <FormGroup>
                        <ControlLabel>{getResource('Creator')}</ControlLabel>
                        <MultiSelect className="react-select-container creator-field" classNamePrefix="react-select"
                            name="creators"
                            value={filter.creators}
                            options={creatorOptions}
                            onChange={this.handleCreatorChange}
                        />
                    </FormGroup>
                </div>
                <div className="row-filter">
                    <FormGroup className="project-tag">
                        <ControlLabel>{getResource('ProjectTag')}</ControlLabel>
                        <Select name="projects" isMulti
                            className="react-select-container project-field" classNamePrefix="react-select"
                            value={filter.projects}
                            options={projectOptions}
                            closeMenuOnSelect={false}
                            onChange={this.handleProjectChange}
                        />
                    </FormGroup>
                    <FormGroup>
                        <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                    </FormGroup>
                </div>
            </Form>
        );
    }
}

export default SearchForm;