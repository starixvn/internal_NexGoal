import React, { Component, Fragment } from 'react';
import { Form, FormGroup, ControlLabel, FormControl, Button, Row, Col } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Select from 'react-select';

import commonFunctions from '../../../common-functions';
import MultiSelect from '../../../shared/components/MultiSelect';

class Filter extends Component {
    constructor(props) {
        super(props);

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
            <Fragment>
                <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                    <div className="row-filter">
                        <FormGroup>
                            <ControlLabel>From Date</ControlLabel>
                            <DatePicker dateFormat="DD MMM, YYYY" selected={filter.fromDate}
                                onChange={this.onFromDateChange} onChangeRaw={this.onChangeRaw}
                                name="fromDate" className="form-control" autoComplete="off"
                            />
                        </FormGroup>
                        <FormGroup>
                            <ControlLabel>To Date</ControlLabel>
                            <DatePicker dateFormat="DD MMM, YYYY" selected={filter.toDate}
                                onChange={this.onToDateChange} onChangeRaw={this.onChangeRaw}
                                name="toDate" className="form-control" autoComplete="off"
                            />
                        </FormGroup>
                        <FormGroup>
                            <ControlLabel>Creator</ControlLabel>
                            <MultiSelect name="creators" value={filter.creators}
                                closeMenuOnSelect={false}
                                options={creatorOptions}
                                onChange={this.handleCreatorChange}
                            />
                        </FormGroup>
                    </div>
                    <div className="row-filter">
                        <FormGroup className="project-tag">
                            <ControlLabel>Project Tag</ControlLabel>
                            <Select name="projects" isMulti
                                className="react-select-container project-field" classNamePrefix="react-select"
                                value={filter.projects}
                                options={projectOptions}
                                closeMenuOnSelect={false}
                                onChange={this.handleProjectChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Button type="submit" bsStyle="primary" className="btn-primary">Search</Button>
                        </FormGroup>
                    </div>
                </Form>
                <a href={`${commonFunctions.getRootUrl()}Self/PerformanceCheckIn/Create`} className="btn btn-primary btn-add">
                    <span className="icon-playlist-plus" />Performance Check-in
                </a>
            </Fragment>
        );
    }
}

export default Filter;