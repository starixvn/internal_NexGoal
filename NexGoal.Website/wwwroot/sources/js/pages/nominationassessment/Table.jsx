import React, { Component, Fragment } from 'react';
import Select, { components } from 'react-select';

import getResource from '../../resourceHelper';
import { NominationStatuses } from '../../constants';
import { getNominationClassName } from '../../common-functions';

const SingleValue = ({ children, ...props }) => (
    <components.SingleValue {...props}>
        <span className={getNominationClassName(props.selectProps.value.value)} >{children}</span>
    </components.SingleValue>
);

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominationStatuses: NominationStatuses.map(x => ({
                value: x.value,
                label: x.text
            }))
        }

        this.selectComponents = { SingleValue };
    }

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(nomination, performanceRecordId);
    }

    handleChangeStatus = (nominationId, status) => {
        this.props.onUpdateStatus(nominationId, status.value);
    }

    renderNominator = nominator => (
        <tr key={nominator.performanceRecordId}>
            <td>{`${nominator.creator.fullName} (${nominator.creator.nickName})`}</td>
            <td className="text-center">
                <a className="fa fa-search" onClick={this.handleShowNominatorComment.bind(this, nominator)}></a>
            </td>
        </tr>
    )

    renderTableData = (item) => {
        return (
            <Fragment key={item.period.nominationPeriodId}>
                <tr>
                    <td colSpan={6}>{item.period.title}</td>
                </tr>
                {item.nominations.map((n, index) => this.renderNomination(n, index, item.canUpdateNominationStatus))}
            </Fragment>
        );
    }

    renderNomination = (nomination, index, canUpdate) => {
        let renderStatus = canUpdate ?
            <Select name="nominationPeriod" className="react-select-container" classNamePrefix="react-select"
                value={this.state.nominationStatuses.find(x => x.value === nomination.statusId)} options={this.state.nominationStatuses}
                isClearable={false} isSearchable={false} components={this.selectComponents}
                onChange={this.handleChangeStatus.bind(this, nomination.nominationId)}
            />
            :
            <span className={getNominationClassName(nomination.statusId)}>
                {NominationStatuses.find(x => x.value === nomination.statusId).text}
            </span>

        return (
            <tr key={nomination.nominationId}>
                <td className="text-center">{index + 1}</td>
                <td>{`${nomination.nominee.fullName} (${nomination.nominee.nickName})`}</td>
                <td>{nomination.nominee.groupName}</td>
                <td className="text-center">{nomination.nominators.length}</td>
                <td>
                    {nomination.nominators.map((n, i, arr) =>
                        <Fragment key={n.performanceRecordId}>
                            <a onClick={this.handleShowNominatorComment.bind(this, nomination, n.performanceRecordId)}>{`${n.creator.fullName} (${n.creator.nickName})`}</a>
                            {i < arr.length - 1 && ', '}
                        </Fragment>)
                    }
                </td>
                <td>{renderStatus}</td>
            </tr>
        )
    }

    render() {
        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>{getResource('SymbolCount')}</th>
                        <th>{getResource('Nominee')}</th>
                        <th>{getResource('Team')}</th>
                        <th>{getResource('Vote')}</th>
                        <th>{getResource('Nominator')}</th>
                        <th>{getResource('Status')}</th>
                    </tr>
                </thead>
                <tbody>
                    {this.props.tableData.map(this.renderTableData)}
                </tbody>
            </table>
        );
    }
}