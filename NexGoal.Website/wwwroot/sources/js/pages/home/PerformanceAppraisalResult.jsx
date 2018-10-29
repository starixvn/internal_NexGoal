import React, { Component } from 'react';
import moment from 'moment';

export default class PerformanceAppraisalResult extends Component {
    constructor(props) {
        super(props);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.performanceAppraisals) !== JSON.stringify(nextProps.performanceAppraisals)) {
            return true;
        }
        return false;
    }

    groupPerformanceAppraisals = (pa) => {
        let performanceAppraisals = pa;
        performanceAppraisals.forEach(record => record.year = moment(record.period.startDate).year());
        performanceAppraisals = performanceAppraisals.reduce(function (groups, item) {
            const val = item['year'];
            groups[val] = groups[val] || [];
            groups[val].push(item);

            return groups;
        }, []);

        performanceAppraisals = Object.keys(performanceAppraisals).map(key => {
            return {
                year: key,
                records: performanceAppraisals[key]
            };
        });

        return performanceAppraisals;
    };

    render() {
        const performanceAppraisals = this.groupPerformanceAppraisals(this.props.performanceAppraisals);

        return (
            <div className="my-pa box">
                <h5>My PAs</h5>
                {
                    performanceAppraisals.map(performanceAppraisal => {
                        return (
                            <div key={`year${performanceAppraisal.year}`} className="my-pa-year">
                                <div className="year">2018</div>
                                {
                                    performanceAppraisal.records.map(record => {
                                        let grade = record.grade ? record.grade.name : '';

                                        return (
                                            <div key={`performanceAppraisal${record.paRecordId}`} className="term">
                                                <div>{record.period.name}</div>
                                                <div className="grade">{grade}</div>
                                            </div>
                                        );
                                    })
                                }
                            </div>
                        );
                    })
                }
            </div>
        );
    }
}