import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';

class Filter extends Component {
    constructor(props) {
        super(props);
    }

    handleYearChange(event) {
        this.props.handleYearChange(event.target.value);
    }

    handleStatusChange(event) {
        this.props.handleStatusChange(event.target.value);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.handleSubmit();
    }

    render() {
        const years = this.props.years;
        const year = this.props.year;
        const statusId = this.props.statusId;

        return (
            <form className="from-inline form-filter" onSubmit={this.handleSubmit.bind(this)}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>Year</ControlLabel>
                        <div className="control-content">
                            <select className="form-control" name="Year" value={year}
                                onChange={this.handleYearChange.bind(this)}>
                                <option key="option-0" value="">-- All --</option>
                                {
                                    years.map((option) => {
                                        return (
                                            <option key={'option-' + option} value={option}>{option}</option>
                                        )
                                    })
                                }
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>Status</ControlLabel>
                        <div className="control-content">
                            <select className="form-control" name="StatusId" value={statusId}
                                onChange={this.handleStatusChange.bind(this)}>
                                <option value="">-- All --</option>
                                <option className="nomination-period-status not-started" value="0">Not Ready</option>
                                <option className="nomination-period-status in-progress" value="1">In Progress</option>
                                <option className="nomination-period-status closed" value="2">Closed</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <button className="btn btn-primary">Search</button>
                    </div>
                </FormGroup>
            </form>
        );
    }
}

export default Filter;