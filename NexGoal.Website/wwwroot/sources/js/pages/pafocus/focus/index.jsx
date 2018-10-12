import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import SearchForm from './SearchForm';
import EvaluationProcess from './EvaluationProcess';
import Table from './Table';

export default class PAFocus extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                period: null,
                groups: [],
                employee: null
            },
            isHost: false,
            focusEvaluationLevelOptions: [],
            periodOptions: [],
            groupOptions: [],
            memberOptions: [],
            periodSelected: null,
            pafocuses: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('pafocus/getPeriod')),
            axios(resolveClientUrl('pafocus/getGroup')),
            axios(resolveClientUrl('pafocus/getMember')),
            axios(resolveClientUrl('pafocus/getFocusEvaluationLevel'))
        ];

        Promise.all(promises)
            .then(([periodResponse, groupResponse, memberResponse, levelResponse]) => {
                const periodOptions = periodResponse.data.map((period) => {
                    return {
                        value: period.periodId,
                        label: period.name,
                        focusStartDate: period.focusStartDate,
                        focusEndDate: period.focusEndDate
                    };
                });

                const groupOptions = groupResponse.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                let memberOptions = [];
                memberResponse.data.map(item => {
                    memberOptions.push({
                        value: item.employeeId,
                        label: `${item.fullName} (${item.nickName})`
                    });
                });

                const filter = Object.assign({}, this.state.filter, {
                    period: periodOptions[0],
                    groups: groupOptions
                });

                this.getPAFocus(filter);

                this.setState({
                    filter: filter,
                    focusEvaluationLevelOptions: levelResponse.data,
                    periodOptions: periodOptions,
                    groupOptions: groupOptions,
                    memberOptions: memberOptions
                });
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    getPAFocus = (filter) => {
        const groupIds = filter.groups.map((group) => { return group.value; }).join(',');

        axios(resolveClientUrl('pafocus/getPAFocus'), {
            params: {
                periodId: filter.period.value,
                groupIds: groupIds,
                employeeId: filter.employee ? filter.employee.value : ''
            }
        }).then(response => {
            this.setState({
                periodSelected: filter.period,
                isHost: response.data.isHost,
                pafocuses: response.data.paFocuses
            });
        }).catch(axiosError);
    }

    handleFilterSubmit = () => {
        let filter = this.state.filter;
        this.getPAFocus(filter);
    }

    handleChange = (groupCode, paFocusId, propName, value) => {
        let pafocuses = this.state.pafocuses.slice();
        let index = pafocuses.map(pafocus => { return pafocus.groupCode; }).indexOf(groupCode);

        if (index >= 0) {
            let memberIndex = pafocuses[index].members.map(member => { return member.paFocusId; }).indexOf(paFocusId);

            if (memberIndex >= 0) {
                pafocuses[index].members[memberIndex] = Object.assign({}, pafocuses[index].members[memberIndex], { [propName]: value });
                this.setState({ pafocuses });
            }
        }
    }

    handleSubmit = () => {
        let pafocuses = [];

        this.state.pafocuses.forEach((pafocus) => {
            pafocus.members.forEach((member) => {
                if (member.isHost) {
                    pafocuses.push({
                        paFocusId: member.paFocusId,
                        isReadyPromotion: member.isReadyPromotion,
                        contribution: member.contribution,
                        bonusGiven: member.bonusGiven,
                        currentObjective: member.currentObjective,
                        nextObjective: member.nextObjective
                    });
                }
            });
        });

        if (pafocuses.length === 0) {
            return;
        }

        axios({
            url: resolveClientUrl('pafocus/edit'),
            method: 'POST',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paFocuses: pafocuses
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        let period = this.state.filter.period,
            today = moment(new Date()),
            focusStartDate = period && period.focusStartDate ? moment(period.focusStartDate) : null,
            focusEndDate = period && period.focusEndDate ? moment(period.focusEndDate) : null;

        return (
            <Fragment>
                <SearchForm filter={this.state.filter}
                    periodOptions={this.state.periodOptions}
                    groupOptions={this.state.groupOptions}
                    memberOptions={this.state.memberOptions}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleFilterSubmit}
                />

                <EvaluationProcess period={this.state.periodSelected} />

                <Table
                    data={this.state.pafocuses}
                    focusEvaluationLevelOptions={this.state.focusEvaluationLevelOptions}
                    onChange={this.handleChange}
                />

                {
                    this.state.isHost && focusStartDate <= today && focusEndDate >= today &&
                    <div className="navbar navbar-default navbar-fixed-bottom action-section text-center">
                        <button className="btn btn-cancel" onClick={this.handleFilterSubmit}>{getResource('Reset')}</button>
                        <button className="btn btn-primary" onClick={this.handleSubmit}>{getResource('Submit')}</button>
                    </div>
                }
            </Fragment>
        );
    }
}