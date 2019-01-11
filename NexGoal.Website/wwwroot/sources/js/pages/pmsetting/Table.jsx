import React, { Component, Fragment } from 'react';

import getResource from '../../resourceHelper';
import { NominationStatuses } from '../../constants';
import { getNominationClassName, noDataIndication } from '../../common-functions';

class Row extends Component {
    constructor(props) {
        super(props);

        this.state = {
            expand: true
        }
    }

    toggleExpand = () => this.setState({ expand: !this.state.expand })

    handleAddMember = pm => this.props.onAddMember(pm)

    handleRemoveMember = member => this.props.onRemoveMember(member)

    renderMember = (member) =>
        <tr className={this.state.expand ? '' : 'hidden'}>
            <td>{member.nickName} ({member.fullName})</td>
            <td>{member.jobTitle}</td>
            <td className='text-center'><a className="icon-delete" onClick={this.handleRemoveMember.bind(this, member)} title="Remove"></a></td>
        </tr>

    render() {
        let data = this.props.data;

        return (
            <Fragment >
                <tr>
                    <td colSpan={3}>
                        <span>{data.pm.nickName} ({data.members.length} members)</span>
                        <a className="icon-plus" title="Add" onClick={this.handleAddMember.bind(this, data.pm)}></a>
                        <a className={`icon-chevron-down ${this.state.expand ? 'collapsed' : 'expanded'}`} title="Collapse/Expand" onClick={this.toggleExpand}></a>
                    </td>
                </tr>
                {data.members.map(this.renderMember)}
            </Fragment>
        );
    }
}

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    renderSetting = (data) => <Row key={data.pm.employeeId} data={data} onAddMember={this.props.onAddMember} onRemoveMember={this.props.onRemoveMember} />

    render() {
        let content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderSetting)
            : <tr><td className="text-center" colSpan={3}>{noDataIndication()}</td></tr>;

        return (
            <table className="table table-bordered table-striped tbl-pm-assignment">
                <thead>
                    <tr>
                        <th>{getResource('Employee')}</th>
                        <th>{getResource('JobTitle')}</th>
                        <th className="text-center col-120">Remove</th>
                    </tr>
                </thead>
                <tbody>
                    {content}
                </tbody>
            </table>
        );
    }
}