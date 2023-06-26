import React, { Component } from 'react';
import moment from 'moment';
import isEqual from 'react-fast-compare';

import getResource from '../../resourceHelper';

export default class PerformanceAppraisalResult extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    groupPerformanceAppraisals = performanceAppraisals => {
        performanceAppraisals.forEach(record => record.year = moment(record.period.startDate).year());
        const pAs = performanceAppraisals.reduce(function (groups, item) {
            const val = item['year'];
            groups[val] = groups[val] || [];
            groups[val].push(item);

            return groups;
        }, []);

        const performanceAppraisalResults = Object.keys(pAs).map(key => {
            return {
                year: key,
                records: pAs[key].sort(function (a, b) { return new Date(b.period.startDate) - new Date(a.period.startDate); })
            };
        });

        return performanceAppraisalResults.sort(function (a, b) { return b.year - a.year; });
    };

    render() {
        const performanceAppraisals = this.props.performanceAppraisals ? this.groupPerformanceAppraisals(this.props.performanceAppraisals) : [];
        const title = this.props.title ? this.props.title : getResource('MyPerformanceAppraisal');

        return (
            <div className="my-pa box">
                <h5>{title}</h5>
                {
                    performanceAppraisals.length === 0 &&
                    <div>{getResource('NoRecordFound')}</div>
                }
                <div className="my-pa-list">
                    {
                        performanceAppraisals.length > 0 &&
                        performanceAppraisals.map(performanceAppraisal => {
                            return (
                                <div key={`year${performanceAppraisal.year}`} className="my-pa-year">
                                    <div className="year">{performanceAppraisal.year}</div>
                                    {
                                        performanceAppraisal.records.map(record => {
                                            const grade = record.grade ? record.grade.description : '';
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
            </div>
        );
    }
}