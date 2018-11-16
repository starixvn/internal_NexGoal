import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../../common-functions';
import { nominationRoleOptions } from '../../common';
import getResource from '../../../../resourceHelper';
import Filter from './Filter';
import Table from './Table';

export default class RoleManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoyId: document.getElementById('EOYId').value,
            filter: {
                keyword: '',
                groups: [],
                role: nominationRoleOptions[0],
                isShowAll: false
            },
            groupOptions: [],
            nominators: [],
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentDidMount() {
        this.getGroups();
        this.getNominatorSetting();
    }

    getGroups = () => {
        axios(resolveClientUrl('Common/GetGroups'))
            .then(response => {
                const groupOptions = response.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                this.setState({
                    groupOptions: groupOptions,
                    filter: Object.assign({}, this.state.filter, { groups: groupOptions })
                });
            }).catch(axiosError);
    }

    getNominatorSetting = () => {
        const filter = this.state.filter;
        const groupIds = this.state.filter.groups.map((group) => { return group.value; }).join(',');

        axios.get(resolveClientUrl('EOY/GetNominatorSetting'), {
            params: {
                eoyId: this.state.eoyId,
                keyword: filter.keyword,
                groupIds: groupIds,
                roleId: filter.role.roleId,
                isShowAll: filter.isShowAll
            }
        }).then(response => {
            this.setState({ nominators: response.data });
        }).catch(axiosError);
    }

    editNominatorSetting = (nominator) => {
        const setting = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoqyId: this.state.eoyId,
            employeeId: nominator.employeeId,
            roleIds: nominator.roles.join(',')
        });

        axios({
            url: resolveClientUrl('EOY/EditNominatorSetting'),
            method: 'POST',
            data: setting,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));

                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleSubmit = () => {
        this.getNominatorSetting();
    }

    render() {
        return (
            <Fragment>
                <Filter filter={this.state.filter}
                    groupOptions={this.state.groupOptions}
                    nominationRoleOptions={nominationRoleOptions}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleSubmit}
                />

                <Table isHost={this.state.isHost}
                    nominators={this.state.nominators}
                    onEdit={this.editNominatorSetting}
                />
            </Fragment>
        );
    }
}