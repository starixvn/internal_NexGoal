import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import commonFunctions from '../../../../common-functions';
import { nominationRoleOptions } from '../../common';
import Filter from './Filter';
import Table from './Table';

export default class RoleManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoqId: document.getElementById('EOQId').value,
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

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.editNominatorSetting = this.editNominatorSetting.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        this.getGroups();
        this.getNominatorSetting();
    }

    getGroups() {
        axios(commonFunctions.getRootUrl() + 'Common/GetGroups')
            .then(response => {
                const groupOptions = response.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                this.setState({
                    groupOptions: groupOptions,
                    filter: Object.assign({}, this.state.filter, { groups: groupOptions })
                });
            }).catch(commonFunctions.axiosError);
    }

    getNominatorSetting() {
        const groupIds = this.state.filter.groups.map((group) => { return group.value; }).join(',');

        axios.get(commonFunctions.getRootUrl()
            + `EOQ/GetNominatorSetting?eoqId=${this.state.eoqId}`
            + `&keyword=${this.state.filter.keyword}`
            + `&groupIds=${groupIds}`
            + `&roleId=${this.state.filter.role.value}`
            + `&isShowAll=${this.state.filter.isShowAll}`)
            .then(response => {
                const data = response.data;
                data.map((e, index) => e.rowNum = index + 1);

                this.setState({ nominators: data });
            }).catch(commonFunctions.axiosError);
    }

    editNominatorSetting(nominator) {
        const setting = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoqyId: this.state.eoqId,
            employeeId: nominator.employeeId,
            roleIds: nominator.roles.join(',')
        });

        axios({
            url: commonFunctions.getRootUrl() + 'EOQ/EditNominatorSetting',
            method: 'POST',
            data: setting,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated successfully!');

                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    handleFilterChange(filter) {
        this.setState({ filter: filter });
    }

    handleSubmit() {
        this.getNominatorSetting();
    }

    render() {
        return (
            <Fragment>
                <Filter filter={this.state.filter}
                    groupOptions={this.state.groupOptions}
                    nominationRoleOptions={nominationRoleOptions}
                    handleFilterChange={this.handleFilterChange}
                    handleFilterSubmit={this.handleSubmit} />

                <Table isHost={this.state.isHost}
                    nominators={this.state.nominators}
                    editNominatorSetting={this.editNominatorSetting} />
            </Fragment>
        );
    }
}