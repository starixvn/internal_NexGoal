import React, { Component, Fragment } from 'react';
import { Collapse } from 'react-bootstrap';

export default class Row extends Component {
    constructor(props) {
        super(props);

        this.state = {
            expand: true
        };
    }

    toggleExpand = () => this.setState({ expand: !this.state.expand })

    handleAddMember = pm => this.props.onAddMember(pm)

    handleRemoveMember = member => this.props.onRemoveMember(member)

    renderMember = (member) => {
        return (
            <Collapse key={member.employeeId} in={this.state.expand}>
                <tr>
                    <td>{member.nickName} ({member.fullName})</td>
                    <td>{member.jobTitle}</td>
                    <td className='text-center'>
                        <a className="icon-delete" onClick={this.handleRemoveMember.bind(this, member)} title="Remove" />
                    </td>
                </tr>
            </Collapse>
        );
    }

    render() {
        let data = this.props.data;

        return (
            <Fragment >
                <tr>
                    <td colSpan={3}>
                        <span>{data.pm.nickName} ({data.members.length} members)</span>
                        <a className="icon-plus" title="Add" onClick={this.handleAddMember.bind(this, data.pm)} />
                        <a className={`icon-chevron-down ${this.state.expand ? 'collapsed' : 'expanded'}`}
                            title="Collapse/Expand"
                            onClick={this.toggleExpand}
                        />
                    </td>
                </tr>
                {data.members.map(this.renderMember)}
            </Fragment>
        );
    }
}