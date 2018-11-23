import React, { Component, Fragment } from 'react';
import Table from './table/Table';

export default class PerformanceAppraisalDetail extends Component {
    render() {
        return (
            <Fragment>
                <Table data={this.props.data} gradeOptions={this.props.gradeOptions} />
            </Fragment>
        );
    }
}