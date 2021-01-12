import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { PostRequestHeaders } from '../../../constants';
import getResource from '../../../resourceHelper';
import SearchForm from './SearchForm';
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
            pafocuses: [],
            isSubmittingForm: true
        };

        this.periodOptions = [];
        this.memberOptions = [];
        this.groupOptions = [];
        this.focusEvaluationLevelOptions = [];
        this.periodSelected = null;
        this.isHost = false;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('pafocus/getPeriod')),
            axios(resolveClientUrl('pafocus/getFocusEvaluationLevel')),
            axios(resolveClientUrl('pafocus/getGroup'))
        ];

        Promise.all(promises)
            .then(([periodResponse, levelResponse, groupResponse]) => {
                const periodSelections = periodResponse.data.map(period => {
                    return {
                        value: period.periodId,
                        label: period.name,
                        focusStartDate: period.focusStartDate,
                        focusEndDate: period.focusEndDate
                    };
                });

                const groupSelections = groupResponse.data.map(group => {
                    return { value: group.groupId, label: group.name };
                });

                const filter = Object.assign({}, this.state.filter, {
                    period: periodSelections[0],
                    groups: groupSelections
                });

                const groupIds = groupSelections && groupSelections.length > 0 ? groupSelections.map(group => group.value).join(',') : null;
                this.getMembers(groupIds);
                this.getPAFocus(filter);

                this.periodOptions = periodSelections;
                this.groupOptions = groupSelections;
                this.focusEvaluationLevelOptions = levelResponse.data;
                this.setState({ filter: filter });
            }).catch(axiosError);

        window.addEventListener('beforeunload', this.warnUnsavedData);
    }

    componentWillUnmount() {
        window.removeEventListener('beforeunload', this.warnUnsavedData);
    }

    warnUnsavedData = event => {
        if (this.state.isSubmittingForm) {
            return undefined;
        }

        const confirmationMessage = 'It looks like you have been editing something. '
            + 'If you leave before saving, your changes will be lost.';

        (event || window.event).returnValue = confirmationMessage;

        return confirmationMessage;
    };

    getMembers = groupIds => {
        axios(resolveClientUrl('pafocus/getMember'), {
            params: {
                groupIds: groupIds
            }
        }).then(response => {
            const memberOptions = response.data.map(item => {
                return {
                    value: item.employeeId,
                    label: `${item.nickName} (${item.fullName})`
                };
            });

            let filter = this.state.filter;

            if (memberOptions.indexOf(filter.employee) < 0) {
                filter = Object.assign({}, this.state.filter, { employee: null });
            }

            this.memberOptions = memberOptions;
            this.setState({ filter: filter });
        }).catch(axiosError);
    };

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        }, () => {
            if (name === 'groups') {
                const groupIds = value && value.length > 0 ? value.map(group => group.value).join(',') : null;

                this.getMembers(groupIds);
            }
        });
    };

    getPAFocus = filter => {
        const periodId = filter.period ? filter.period.value : null;
        const groupIds = filter.groups.map(group => group.value).join(',');

        axios(resolveClientUrl('pafocus/getPAFocus'), {
            params: {
                periodId: periodId,
                groupIds: groupIds,
                employeeId: filter.employee ? filter.employee.value : ''
            }
        }).then(response => {
            this.isHost = response.data.isHost;
            this.periodSelected = filter.period;

            this.setState({
                pafocuses: response.data.paFocuses,
                isSubmittingForm: true
            });
        }).catch(axiosError);
    };

    handleFilterSubmit = () => {
        const filter = this.state.filter;
        this.getPAFocus(filter);
    };

    handleExportToExcel = () => {
        const filter = this.state.filter;
        const groupIds = filter.groups.map(group => group.value).join(',');
        const employeeId = filter.employee ? filter.employee.value : '';

        axios({
            url: resolveClientUrl('pafocus/ExportToExcel') + `?periodId=${filter.period.value}&groupIds=${groupIds}&employeeId=${employeeId}`,
            method: 'get',
            responseType: 'blob'
        }).then(response => {
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `PA Focus of ${filter.period ? filter.period.label : ''}.xlsx`);
            document.body.appendChild(link);
            link.click();
        });
    };

    handleChange = (groupCode, paFocusId, propName, value) => {
        const pafocuses = this.state.pafocuses.slice();
        const index = pafocuses.findIndex(pafocus => pafocus.groupCode === groupCode);

        if (index >= 0) {
            const memberIndex = pafocuses[index].members.findIndex(member => member.paFocusId === paFocusId);

            if (memberIndex >= 0) {
                pafocuses[index].members[memberIndex] = Object.assign({}, pafocuses[index].members[memberIndex], { [propName]: value });
                this.setState({
                    pafocuses: pafocuses,
                    isSubmittingForm: false
                });
            }
        }
    };

    handleSubmit = () => {
        const pafocuses = [];

        this.state.pafocuses.forEach(pafocus => {
            pafocus.members.forEach(member => {
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
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(data.message);
            }

            this.setState({
                isSubmittingForm: true
            });
        }).catch(axiosError);
    };

    render() {
        const period = this.state.filter.period,
            today = moment(new Date()).startOf('day'),
            focusStartDate = period && period.focusStartDate ? moment(period.focusStartDate).startOf('day') : null,
            focusEndDate = period && period.focusEndDate ? moment(period.focusEndDate).startOf('day') : null;

        return (
            <Fragment>
                <SearchForm filter={this.state.filter}
                    periodOptions={this.periodOptions}
                    groupOptions={this.groupOptions}
                    memberOptions={this.memberOptions}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleFilterSubmit}
                    onExportToExcel={this.handleExportToExcel}
                />

                <Table
                    data={this.state.pafocuses}
                    period={this.periodSelected}
                    focusEvaluationLevelOptions={this.focusEvaluationLevelOptions}
                    onChange={this.handleChange}
                    isDisplayButton={this.isHost && focusStartDate <= today && focusEndDate >= today}
                    handleFilterSubmit={this.handleFilterSubmit}
                    handleSubmit={this.handleSubmit}
                    onExportToExcel={this.handleExportToExcel}
                />
            </Fragment>
        );
    }
}