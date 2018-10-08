import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';

export default class PerformanceAppraisalResult extends Component {
    static propTypes = {
        performanceAppraisals: PropTypes.arrayOf(PropTypes.object)
    };

    static defaultProps = {
        performanceAppraisals: []
    };

    render() {
        const performanceAppraisals = this.props.performanceAppraisals;

        return (
            <table className="table table-bordered">
                <thead>
                    <tr><th colSpan="2">My PAs</th></tr>
                </thead>
                <tbody>
                    {
                        performanceAppraisals.map(performanceAppraisal => {
                            return (
                                <Fragment key={`year${performanceAppraisal.year}`}>
                                    <tr>
                                        <td colSpan="2">{performanceAppraisal.year}</td>
                                    </tr>

                                    {
                                        performanceAppraisal.records.map(record => {
                                            let grade = record.grade ? record.grade.name : '';
                                            return (
                                                <tr key={`performanceAppraisal${record.paRecordId}`}>
                                                    <td>{record.period.name}</td>
                                                    <td>{grade}</td>
                                                </tr>
                                            );
                                        })
                                    }
                                </Fragment>
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}