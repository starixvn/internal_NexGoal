import React, { Component } from 'react';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';

export default class PerformanceAppraisalResult extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceAppraisals: []
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('home/GetPerformanceAppraisalResult'))
            .then(response => {
                let performanceAppraisals = response.data;
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

                this.setState({
                    performanceAppraisals: performanceAppraisals
                });
            }).catch(axiosError);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.state.performanceAppraisals) !== JSON.stringify(nextState.performanceAppraisals)) {
            return true;
        }

        return false;
    }

    render() {
        const performanceAppraisals = this.state.performanceAppraisals;

        return (
            <div className="my-pa">
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