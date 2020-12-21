import React, { Component } from 'react';
import axios from 'axios';
import isEqual from 'react-fast-compare';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Filter from './filter';
import Table from './table/Table';
import { PAEditOptions } from '../../../constants';

export default class MemberPerformanceAppraisal extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                period: props.periodOptions[0] || [],
                groups: props.groupOptions || [],
                status: PAEditOptions.find(option => option.value === '')
            },
            performanceAppraisals: null
        };
    }

    componentDidMount() {
        this.getPerformanceAppraisals(this.state.filter);
    }

    shouldComponentUpdate(nextProps, nextState) {
        return !isEqual(this.state, nextState) || !isEqual(this.props, nextProps);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    };

    handleFilterSubmit = () => {
        this.getPerformanceAppraisals();
    };

    onQuickEditSuccess = (paRecordId, isRevised) => {
        const paString = JSON.stringify(this.state.performanceAppraisals);
        const groupPAs = JSON.parse(paString);
        for (let i = 0; i < groupPAs.length; i++) {
            const paIndex = groupPAs[i].performanceAppraisals.findIndex(pa => pa.paRecordId === paRecordId);
            if (paIndex !== -1) {
                groupPAs[i].performanceAppraisals[paIndex].isRevised = !isRevised;
            }
        }

        this.setState({
            performanceAppraisals: groupPAs
        });
    };

    getPerformanceAppraisals = () => {
        const filter = this.state.filter;
        axios.get(resolveClientUrl('Review/GetPARecords'), {
            params: {
                periodId: filter.period.value,
                groupIds: filter.groups.map(group => group.value).join(','),
                isRevised: filter.status.value
            }
        }).then(response => {
            this.setState({
                performanceAppraisals: response.data
            });
        }).catch(axiosError);
    };

    render() {
        return (
            <div className="pa-content">
                <Filter
                    filter={this.state.filter}
                    periodOptions={this.props.periodOptions}
                    groupOptions={this.props.groupOptions}
                    onFilterChange={this.handleFilterChange}
                    onSubmit={this.handleFilterSubmit}
                />
                <Table
                    data={this.state.performanceAppraisals}
                    gradeOptions={this.props.gradeOptions}
                    renderPA={this.getPerformanceAppraisals}
                    onRevisedSuccess={this.onQuickEditSuccess}
                    performanceGradeInfos={this.props.performanceGradeInfos}
                />
            </div>
        );
    }
}