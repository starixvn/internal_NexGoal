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
            axios(resolveClientUrl('pafocus/getFocusEvaluationLevel'))
        ];

        Promise.all(promises)
            .then(([periodResponse, groupResponse, levelResponse]) => {
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

                const filter = Object.assign({}, this.state.filter, {
                    period: periodOptions[0],
                    groups: groupOptions
                });

                const groupIds = groupOptions && groupOptions.length > 0 ? groupOptions.map(group => { return group.value; }).join(',') : null;
                this.getMembers(groupIds);
                this.getPAFocus(filter);

                this.setState({
                    filter: filter,
                    focusEvaluationLevelOptions: levelResponse.data,
                    periodOptions: periodOptions,
                    groupOptions: groupOptions
                });
            }).catch(axiosError);
    }

    getMembers = (groupIds) => {
        axios(resolveClientUrl('pafocus/getMember'), {
            params: {
                groupIds: groupIds
            }
        }).then(response => {
            let memberOptions = response.data.map(item => {
                return {
                    value: item.employeeId,
                    label: `${item.nickName} (${item.fullName})`
                };
            });

            let filter = this.state.filter;

            if (memberOptions.indexOf(filter.employee) < 0) {
                filter = Object.assign({}, this.state.filter, { employee: null });
            }

            this.setState({
                filter: filter,
                memberOptions: memberOptions
            });
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        }, () => {
            if (name === 'groups') {
                const groupIds = value && value.length > 0 ? value.map(group => { return group.value; }).join(',') : null;

                this.getMembers(groupIds);
            }
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

    handleExportToExcel = () => {
        let filter = this.state.filter;
        let groupIds = filter.groups.map((group) => { return group.value; }).join(',');
        let employeeId = filter.employee ? filter.employee.value : '';

        axios({
            url: resolveClientUrl('pafocus/ExportToExcel') + `?periodId=${filter.period.value}&groupIds=${groupIds}&employeeId=${employeeId}`,
            method: 'get',
            responseType: 'blob'
        }).then((response) => {
            let url = window.URL.createObjectURL(new Blob([response.data]));
            let link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `PA Focus of ${filter.period ? filter.period.label : ''}.xlsx`);
            document.body.appendChild(link);
            link.click();
        });
    }

    handleChange = (groupCode, paFocusId, propName, value) => {
        let pafocuses = this.state.pafocuses.slice();
        let index = pafocuses.findIndex(pafocus => pafocus.groupCode === groupCode);

        if (index >= 0) {
            let memberIndex = pafocuses[index].members.findIndex(member => member.paFocusId === paFocusId);

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
                'X-Requested-With': 'XMLHttpRequest',
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
                    onExportToExcel={this.handleExportToExcel}
                />

                {/* <EvaluationProcess period={this.state.periodSelected} /> */}

                <Table
                    data={this.state.pafocuses}
                    focusEvaluationLevelOptions={this.state.focusEvaluationLevelOptions}
                    onChange={this.handleChange}
                />

                {
                    this.state.isHost && focusStartDate <= today && focusEndDate >= today &&
                    <div className="navbar navbar-default navbar-fixed-bottom action-section button-block text-center">
                        <button className="btn btn-cancel" onClick={this.handleFilterSubmit}>{getResource('Reset')}</button>
                        <button className="btn btn-primary" onClick={this.handleSubmit}>{getResource('Submit')}</button>
                    </div>
                }
            </Fragment>
        );
    }
}