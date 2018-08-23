import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleSubmit(event) {
    }

    render() {
        return (
            <form className="from-inline form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>From Date</ControlLabel>
                        <div className="control-content">
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>To Date</ControlLabel>
                        <div className="control-content">
                        </div>
                    </div>
                </FormGroup>
                <FormGroup>
                    <div className="col">
                        <button className="btn btn-primary">Search</button>
                    </div>
                </FormGroup>
            </form>
        );
    }
}

export default Filter;